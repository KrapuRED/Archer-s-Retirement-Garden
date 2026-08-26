using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class UpgradeCardData
{
    public string Name;
    public float change;
    public List<UpgradeCardSO> ListOfUpgradeCardSo = new List<UpgradeCardSO>();
}

[System.Serializable]
public class UpgradeCardPool
{
    public string poolName;
    public int days;
    public List<UpgradeCardData> upgradeCardDatas = new();
}

public class UpgradeCardRunTimeData
{
    public UpgradeCardSO cardSO;
    public int totalBuy;
    public int currentPrice;
}

public class UpgradeCardManager : MonoBehaviour
{
    public static UpgradeCardManager Instance { get; private set; }

    [Header("Upgrade CardUI Configuration")]
    [SerializeField] private Transform upgradeCardContainer;
    [SerializeField] private List<UpgradeCardUI> upgradeCardUIs = new(); 
    
    [Header("Upgrade Card Configuration")]
    [SerializeField] private List<UpgradeCardPool> upgradeCardPools = new();
    [SerializeField] private List<UpgradeCardSO> activeUpgradeCards = new();
    [SerializeField] private int maxActiveRandomCards;
    [SerializeField] private float increasePriceStoryMode;
    [SerializeField] private float increasePriceEndlessMode;

    private readonly Dictionary<UpgradeCardSO, UpgradeCardRunTimeData> _runTimeData = new();
    private readonly HashSet<SkillCardSO> _ownedSkillCards = new();
    
    private int _totalAllUpgrades;
    private bool _isInitialized;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return; 
        }

        Instance = this;
        upgradeCardUIs = upgradeCardContainer.GetComponentsInChildren<UpgradeCardUI>().ToList();
    }
    
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _isInitialized = true;
    }
    
    private float GetNewPrice(int basePrice, int totalBuy)
    {
        float newPrice = basePrice * (1  + totalBuy * (increasePriceStoryMode / 100f));
        return newPrice;
    }

    private UpgradeCardRunTimeData GetOrCreateRunTimeData(UpgradeCardSO so)
    {
        if (!_runTimeData.TryGetValue(so, out UpgradeCardRunTimeData data))
        {
            data = new UpgradeCardRunTimeData
            {
                cardSO = so,
                totalBuy = 0,
                currentPrice = Mathf.RoundToInt(GetNewPrice(so.upgradeBaseCost, _totalAllUpgrades))
            };
            _runTimeData.Add(so, data);
        }
        return data;
    }

    private void ApplyUpgradeEffect(UpgradeCardSO upgradeCardSo)
    {
        switch (upgradeCardSo.upgradeType)
        {
            case UpgradeType.BuffCardUpgrade:
                StatusManager.Instance.ApplyUpgradeCardBoost(upgradeCardSo.upgradeStatusType,  upgradeCardSo.upgradeValue);
                break;
            case UpgradeType.PassiveAbilityUpgrade:
                break;
            case UpgradeType.AbilityCard:
                SkillCardManager.Instance.UnlockSkillCard(upgradeCardSo.linkedSkillCard);
                break;
            case UpgradeType.AbilityCardUpgrade:
                SkillCardManager.Instance.UpgradeSkillCard(upgradeCardSo.linkedSkillCard);
                break;
        }
    }

    private UpgradeCardData PickWeightedCardData(List<UpgradeCardData> pool)
    {
        float total = 0;
        foreach (var d in pool)
            total += d.change;

        float roll = Random.Range(0f, total);
        float cumulative = 0;

        foreach (var d in pool)
        {
            cumulative += d.change;
            if (roll <= cumulative)
                return d;
        }

        return pool[pool.Count - 1]; 
    }

    private bool IsCardAvailable(UpgradeCardSO upgradeCardSo)
    {
        if (upgradeCardSo == null) return false;

        if (upgradeCardSo.oneTimeBuy && _runTimeData.TryGetValue(upgradeCardSo, out UpgradeCardRunTimeData data) &&
            data.totalBuy > 0)
            return false;

        if (upgradeCardSo.upgradeType == UpgradeType.AbilityCardUpgrade)
        {
            return upgradeCardSo.linkedSkillCard != null && _ownedSkillCards.Contains(upgradeCardSo.linkedSkillCard);
        }
        
        return true;
    }
    
    public void OnShowRandomUpgradeCard()
    {
        // Show Random Update Card by Day count foe the pool
        int dayCount = DayCycleManager.Instance.DayCount;
        var poolData = upgradeCardPools.Find(x => x.days >= dayCount);
        if (poolData == null)
        {
            Debug.LogError($"[{name} - (OnShowRandomUpgradeCard)] No pool found for day {dayCount}!");
            return;
        }
        
        float accumlate = 0;
        foreach (var upgradeData in poolData.upgradeCardDatas)
            accumlate += upgradeData.change;
        
        if (accumlate > 100f)
        {
            Debug.LogError($"[{name} - (OnShowRandomUpgradeCard)] Cannot Show Random Upgrade Card the change is {accumlate}!");
            return;
        }
        
        activeUpgradeCards.Clear();
        var remainingBuckets = poolData.upgradeCardDatas.ToList();
        
        int attempts = 0;
        int maxAttempts = maxActiveRandomCards * 30;
        var categoryUpgradeStatusType = new HashSet<string>();

        while (activeUpgradeCards.Count < maxActiveRandomCards && attempts < maxAttempts && remainingBuckets.Count > 0)
        {
            attempts++;

            var picked = PickWeightedCardData(poolData.upgradeCardDatas);
            if (picked == null || picked.ListOfUpgradeCardSo.Count == 0)
            {
                remainingBuckets.Remove(picked);
                continue;
            }
            
            if (categoryUpgradeStatusType.Contains(picked.ListOfUpgradeCardSo[0].upgradeStatusType.ToString()))
            {
                remainingBuckets.Remove(picked);
                continue;
            }

            var candidates = picked.ListOfUpgradeCardSo
                .Where(so => IsCardAvailable(so) && !activeUpgradeCards.Contains(so)).ToList();
            
            if  (candidates.Count == 0)
            {
                remainingBuckets.Remove(picked);
                continue;
            }
            
            var so = candidates[Random.Range(0, candidates.Count)];
            activeUpgradeCards.Add(so);
            categoryUpgradeStatusType.Add(so.upgradeStatusType.ToString());
            remainingBuckets.Remove(picked);
        }

        if (activeUpgradeCards.Count < maxActiveRandomCards)
            Debug.LogWarning($"[{name} - (OnShowRandomUpgradeCard)] Only found {activeUpgradeCards.Count}/{maxActiveRandomCards} cards after {attempts} attempts.");
        
        for (int i = 0; i < maxActiveRandomCards; i++)
        {
            if (upgradeCardUIs[i] == null)
                continue;
            
            if (i < activeUpgradeCards.Count)
            {
                upgradeCardUIs[i].gameObject.SetActive(true);
                upgradeCardUIs[i].InitilizeUpgradeCardUI(GetOrCreateRunTimeData(activeUpgradeCards[i]));
            }
        }
    }
    
    public void OnUpgradeCard(UpgradeCardSO upgradeCardData)
    {
        var data = GetOrCreateRunTimeData(upgradeCardData);
        if (!CurrencyManager.Instance.UseCurrency(data.currentPrice))
            return;
        
        ApplyUpgradeEffect(upgradeCardData);
        
        data.totalBuy++;
        
        foreach (var kvp in _runTimeData)
            _totalAllUpgrades += kvp.Value.totalBuy;

        foreach (var kvp in _runTimeData)
        {
            kvp.Value.currentPrice = Mathf.RoundToInt(GetNewPrice(kvp.Key.upgradeBaseCost, _totalAllUpgrades));
            Debug.Log($"[{kvp.Key.upgradeName}] New Current Price: {kvp.Value.currentPrice}");
        }
    }
}
