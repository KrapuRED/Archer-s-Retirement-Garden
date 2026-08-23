using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public Arrow arrowPrefab;
    
    public abstract void UseSkill(SkillCardData  skillCardData);
}
