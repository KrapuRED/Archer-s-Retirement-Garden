using System.Collections;
using UnityEngine;

public class QuadArrowSkill : Skill
{
    [SerializeField] private Transform[] markers;
    
    public override void UseSkill(SkillCardData  skillCardData)
    {
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardData.currentAttackBoost}");
        foreach (var marker in markers)
        {
            StartCoroutine(ArrowSpawn(marker, skillCardData));
        }
    }
    
    private IEnumerator ArrowSpawn(Transform marker, SkillCardData skillCardData)
    {
        Vector3 spawnPosition = new Vector3(marker.position.x, marker.position.y + offsetSpawnArrow, marker.position.z);
        
        var arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity, this.transform);
        if (arrow == null)
        {
            Destroy(arrow);
        }

        arrow.OnSpawnArrow(skillCardData);
        
        yield return new  WaitForSeconds(skillCardData.currentDuration);
        Destroy(gameObject);
    }
}
