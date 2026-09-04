using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum BoostType
{
    None,
    Attack,
    Critical,
    Health,
    Healing,
    Gold
}

[System.Serializable]
public class BoostStatusData
{
    public string boostName;
    public float boostAmount;
    public BoostType boostType;
    public bool isDailyBoost;
}

[System.Serializable]
public class GardenBoostTrackData
{
    public GardenItemSO gardenItemSo;
    public int stackItemCount;
}

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance { get; private set; }

    [SerializeField] private CharacterSO characterData;
    
    [SerializeField] private float maxHealthBoost;
    [SerializeField] private float attackBoost;
    [SerializeField] private float attackIntervalBoost;
    [SerializeField] private float arrowVelocityBoost;
    [SerializeField] private float criticalBoostRate;
    [SerializeField] private float criticalBoostDamage;

    private readonly List<GardenBoostTrackData> _gardenBoostTrackDataList = new();

    public float MaxHealthBoost => maxHealthBoost;
    public float AttackBoost => attackBoost;
    public float CriticalBoostRate => criticalBoostRate;
    public float CriticalBoostDamage => criticalBoostDamage;
    public float AttackIntervalBoost => attackIntervalBoost;
    public float ArrowVelocityBoost => arrowVelocityBoost;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        HealthManager.Instance.InitializeHealth(characterData.baseMaxHealth);
        attackBoost = characterData.baseAttack;
        attackIntervalBoost = characterData.baseAttackSpeed;
        criticalBoostRate = characterData.baseCritRate;
        criticalBoostDamage = characterData.bassCritDamage;
        arrowVelocityBoost = characterData.baseSpeed;
    }

    #region Event Configuration

    private void OnEnable()
    {
        GameEvents.OnChangeStackGardenObject.AddListener(HandlingBoost);
    }

    private void OnDisable()
    {
        GameEvents.OnChangeStackGardenObject.RemoveListener(HandlingBoost);
    }

    #endregion

    private void HandlingBoost(GardenItemSO gardenItemSo, int stack)
    {
        if (gardenItemSo.gardenBoostItemDatas == null || gardenItemSo.gardenBoostItemDatas.Count == 0)
            return;

        var trackData = _gardenBoostTrackDataList.Find(x => x.gardenItemSo == gardenItemSo);
        if (trackData == null)
        {
            trackData = new GardenBoostTrackData { gardenItemSo = gardenItemSo, stackItemCount = 0 };
            _gardenBoostTrackDataList.Add(trackData);
        }

        if (trackData.stackItemCount > stack)
        {
            RemoveBoosting(gardenItemSo, stack);
        }
        else
        {
            AddBoosting(gardenItemSo, stack);
        }
        
        trackData.stackItemCount = stack;
    }

    private void AddBoosting(GardenItemSO gardenItemData, int stackDelta)
    {
        foreach (var boostData in gardenItemData.gardenBoostItemDatas)
        {
            float amount =  boostData.boostAmount;
            ApplyBoost(boostData.boostType, amount);
        }
    }

    private void RemoveBoosting(GardenItemSO gardenItemData, int stackDelta)
    {
        foreach (var boostData in gardenItemData.gardenBoostItemDatas)
        {
            float amount = -boostData.boostAmount;
            ApplyBoost(boostData.boostType, amount);
        }
    }
    
    private void ApplyBoost(BoostType boostType, float amount)
    {
        switch (boostType)
        {
            case BoostType.Attack:
                attackBoost += amount;
                break;
            case BoostType.Critical:
                criticalBoostDamage += amount;
                break;
            case BoostType.Health:
                maxHealthBoost += amount;
                HealthManager.Instance.MaxHealthHandler(amount);
                break;
        }
    }

    public void ApplyUpgradeCardBoost(UpgradeStatusType upgradeStatusType, float amount)
    {
        switch (upgradeStatusType)
        {
            case UpgradeStatusType.Attack:
                attackBoost += amount;
                break;
            case UpgradeStatusType.AttackInterval:
                attackIntervalBoost += amount;
                break;
            case UpgradeStatusType.ArrowVelocity:
                arrowVelocityBoost += amount;
                break;
            case UpgradeStatusType.CritChance:
                criticalBoostRate += amount;
                break;
            case UpgradeStatusType.CritDamage:
                criticalBoostDamage += amount;
                break;
        }
        
        SkillCardManager.Instance.UpdateBasicAttack(upgradeStatusType, amount);
    }
}
