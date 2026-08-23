using UnityEngine;

public class RainOfArrowSkill : Skill
{
    public override void UseSkill(SkillCardData  skillCardData)
    {
        arrowPrefab.OnSpawnArrow(skillCardData);
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardData.currentAttackBoost}");
    }
}
