using UnityEngine;

[System.Serializable]
public enum SkillDamageType
{
    None,
    Arrow,
    Explosion
}

public enum DamageType
{
    None,
    AreaOfEffect
}

[System.Serializable]
public class ArrowSkillData
{
    public DamageType damageType;
}

[System.Serializable]
public class ExplosionSkillData
{
    public float explosionRadius;
}

[CreateAssetMenu(fileName = "SkillCardSO", menuName = "Skill Card Data/SkillCardSO")]
public class SkillCardSO : ScriptableObject
{
    public string nameSkillCard;
    public string descriptionSkillCard;
    public Sprite iconSkillCard;
    public float arrowVelocity;
    public bool isAuto;
    
    [Header("Skill Card Configuration")]
    public SkillDamageType damageTypeSkillCard;
    public float attackBoostSkillCard;
    public float durationActiveSkillCard;
    public float cooldownSkillCard;
    public int targetSkillCard;
    
    public ArrowSkillData arrowData;
    public ExplosionSkillData explosionData;
    public GameObject prefabSkillTargeting;
}
