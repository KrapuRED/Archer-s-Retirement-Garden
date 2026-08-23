using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public float offsetSpawnArrow;
    public Arrow arrowPrefab;
    
    public abstract void UseSkill(SkillCardData  skillCardData);
}
