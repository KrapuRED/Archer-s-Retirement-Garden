using System.Collections;
using UnityEngine;

public class ExplosionArrowSkill : Skill
{
    [SerializeField] private float duration;
    [SerializeField] private Transform marker;
    
    public override void UseSkill(SkillCardDataRunTime  skillCardDataRunTime)
    {
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardDataRunTime.currentAttackBoost}");
        
        ArrowSpawn(marker, skillCardDataRunTime);
    }
    
    private void ArrowSpawn(Transform marker, SkillCardDataRunTime skillCardDataRunTime)
    {
        Vector3 spawnPosition = new Vector3(marker.position.x, marker.position.y + offsetSpawnArrow, marker.position.z);
        
        var arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity, this.transform);
        if (arrow == null)
        {
            Destroy(arrow);
            return;
        }

        arrow.OnSpawnArrow(skillCardDataRunTime);
        marker.gameObject.SetActive(false);
    }
}
