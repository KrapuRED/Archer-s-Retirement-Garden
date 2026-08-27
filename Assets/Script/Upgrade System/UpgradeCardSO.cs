using UnityEngine;

public enum UpgradeType
{
    None,
    BuffCardUpgrade,
    PassiveAbilityUpgrade,
    AbilityCard,
    AbilityCardUpgrade
}

[System.Serializable]
public enum UpgradeStatusType
{
    None,
    CritChance,
    CritDamage,
    Attack,
    AttackInterval,
    ArrowVelocity
}

[System.Serializable]
public enum UpgradeRarity
{
    Star1,
    Star2,
    Star3
}

[CreateAssetMenu(fileName = "UpgradeCardSO", menuName = "Upgrade Card Data/UpgradeCardSO")]
public class UpgradeCardSO : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;
    public UpgradeRarity rarity;
    public UpgradeType upgradeType;
    public Sprite upgradeIcon;
    public int upgradeBaseCost;
    public int upgradeAbilityIncrease;
    
    public UpgradeStatusType upgradeStatusType;
    public SkillCardSO baseSkillCard;
    public SkillCardSO linkedSkillCard;
    public float upgradeValue;

    public bool oneTimeBuy;
}
