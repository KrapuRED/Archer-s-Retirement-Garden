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

    #region Event System

    private void OnEnable()
    {
        GameEvents.OnRequestOpenPanel.AddListener(OnShowRandomUpgradeCard);
    }

    private void OnDestroy()
    {
        GameEvents.OnRequestOpenPanel.RemoveListener(OnShowRandomUpgradeCard);
    }

    #endregion
    
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
    
    public void OnShowRandomUpgradeCard(PanelType panelType)
    {
        if (panelType != PanelType.Upgrade) return;
        
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
        
        int attempts = 0;
        int maxAttempts = maxActiveRandomCards * 20;
        var categoryUpgradeStatusType = new HashSet<string>();

        while (activeUpgradeCards.Count < maxActiveRandomCards && attempts < maxAttempts)
        {
            attempts++;

            var picked = PickWeightedCardData(poolData.upgradeCardDatas);
            if (picked == null || picked.ListOfUpgradeCardSo.Count == 0)
                continue;
            
            if (categoryUpgradeStatusType.Contains(picked.ListOfUpgradeCardSo[0].upgradeStatusType.ToString()))
                continue;
            
            var so = picked.ListOfUpgradeCardSo[Random.Range(0, picked.ListOfUpgradeCardSo.Count)];
            activeUpgradeCards.Add(so);
            categoryUpgradeStatusType.Add(so.upgradeStatusType.ToString());
        }

        for (int i = 0; i < maxActiveRandomCards; i++)
        {
            if (upgradeCardUIs[i] == null)
                continue;
            
            upgradeCardUIs[i].InitilizeUpgradeCardUI(GetOrCreateRunTimeData(activeUpgradeCards[i]));
        }
        
        if (activeUpgradeCards.Count < maxActiveRandomCards)
            Debug.LogWarning($"[{name} - (OnShowRandomUpgradeCard)] Only found {activeUpgradeCards.Count}/{maxActiveRandomCards} cards after {attempts} attempts.");
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
