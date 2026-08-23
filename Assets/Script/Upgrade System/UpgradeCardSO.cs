using UnityEngine;

public enum UpgradeType
{
    None,
    BuffCardUpgrade,
    PassiveAbilityUpgrade,
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

[CreateAssetMenu(fileName = "UpgradeCardSO", menuName = "Upgrade Card Data/UpgradeCardSO")]
public class UpgradeCardSO : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;
    public UpgradeType upgradeType;
    public Sprite upgradeIcon;
    public int upgradeBaseCost;
    
    public UpgradeStatusType upgradeStatusType;
    public float upgradeValue;
}
