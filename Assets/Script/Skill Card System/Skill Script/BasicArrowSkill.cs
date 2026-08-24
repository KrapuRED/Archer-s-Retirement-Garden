using UnityEngine;

public class BasicArrowSkill : Skill
{
    public override void UseSkill(SkillCardData  skillCardData)
    {
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardData.currentAttackBoost}");
        ArrowSpawn(this.transform, skillCardData);
    }
    
    private void ArrowSpawn(Transform marker, SkillCardData skillCardData)
    {
        Vector3 spawnPosition = new Vector3(marker.position.x, marker.position.y + offsetSpawnArrow, marker.position.z);
        
        var arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity, this.transform);
        if (arrow == null)
        {
            Destroy(arrow);
            return;
        }

        arrow.OnSpawnArrow(skillCardData);
        marker.gameObject.SetActive(false);

    }
}
