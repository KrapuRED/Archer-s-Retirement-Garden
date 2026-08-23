using UnityEngine;

public class ExplosionArrowSkill : Skill
{
    public override void UseSkill(SkillCardData  skillCardData)
    {
        arrowPrefab.SpawnArrow(skillCardData.skillCardSo.explosionData.explosionRadius);
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardData.currentAttackBoost}");
    }
}
