using UnityEngine;

public class QuadArrowSkill : Skill
{
    public override void UseSkill(SkillCardData  skillCardData)
    {
        arrowPrefab.SpawnArrow();
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardData.currentAttackBoost}");
    }
}
