using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardSO", menuName = "Skill Card Data/SkillCardSO")]
public class SkillCardSO : ScriptableObject
{
    public string NameSkillCard;
    public string DescriptionSkillCard;
    public Sprite IconSkillCard;
    
    [Header("Skill Card Configuration")]
    public float AttackBoostSkillCard;
    public float DurationActivekillCard;
    public float CooldownSkillCard;
}
