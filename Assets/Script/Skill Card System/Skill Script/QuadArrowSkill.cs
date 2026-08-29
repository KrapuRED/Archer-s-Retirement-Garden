using System.Collections;
using UnityEngine;

public class QuadArrowSkill : Skill
{
    [SerializeField] private Transform[] markers;
    
    public override void UseSkill(SkillCardDataRunTime  skillCardDataRunTime)
    {
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardDataRunTime.currentAttackBoost}");
        foreach (var marker in markers)
        {
            StartCoroutine(ArrowSpawn(marker, skillCardDataRunTime));
        }
    }
    
    private IEnumerator ArrowSpawn(Transform marker, SkillCardDataRunTime skillCardDataRunTime)
    {
        Vector3 spawnPosition = new Vector3(marker.position.x, marker.position.y + offsetSpawnArrow, marker.position.z);
        
        var arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity, this.transform);
        if (arrow == null)
        {
            Destroy(arrow);
        }

        arrow.OnSpawnArrow(skillCardDataRunTime);
        
        yield return new  WaitForSeconds(skillCardDataRunTime.currentDuration);
        Destroy(gameObject);
    }
}
