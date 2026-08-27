using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class UpgradeCardData
{
    public string nameCardData;
    public float change;
    public List<UpgradeCardSO> listOfUpgradeCardSo = new List<UpgradeCardSO>();
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
    public UpgradeCardSO CardSo;
    public int TotalBuy;
    public int CurrentPrice;
}

public class UpgradeCardManager : MonoBehaviour
{
    public static UpgradeCardManager Instance { get; private set; }

    [Header("Upgrade CardUI Configuration")]
    [SerializeField] private Transform upgradeCardContainer;
    [SerializeField] private List<UpgradeCardUI> upgradeCardUi = new(); 
    
    [Header("Upgrade Card Configuration")]
    [SerializeField] private List<UpgradeCardPool> upgradeCardPools = new();
    [SerializeField] private List<UpgradeCardSO> activeUpgradeCards = new();
    [SerializeField] private int maxActiveRandomCards;
    [SerializeField] private float increasePriceStoryMode;
    [SerializeField] private float increasePriceEndlessMode;

    private readonly Dictionary<UpgradeCardSO, UpgradeCardRunTimeData> _runTimeData = new();
    
    private UpgradeCardPool _selectedPool;
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
        upgradeCardUi = upgradeCardContainer.GetComponentsInChildren<UpgradeCardUI>().ToList();
    }
    
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _isInitialized = true;
    }
    
    private float GetNewPrice(UpgradeCardSO upgradeData, int totalBuy)
    {
        float newPrice = 0;

        if (GameManager.Instance.GameMode == GameMode.Story)
        {
            if (upgradeData.upgradeType == UpgradeType.AbilityCardUpgrade)
                newPrice = upgradeData.upgradeBaseCost * (1 + totalBuy * (upgradeData.upgradeAbilityIncrease + increasePriceStoryMode / 100f));
            else
            {
                newPrice = upgradeData.upgradeBaseCost * (1 + totalBuy * (increasePriceStoryMode / 100f));
            }
        }
        else
        {
            if (upgradeData.upgradeType == UpgradeType.AbilityCardUpgrade)
                newPrice = upgradeData.upgradeBaseCost * (1 + totalBuy * (upgradeData.upgradeAbilityIncrease + increasePriceEndlessMode / 100f));
            else
            {
                newPrice = upgradeData.upgradeBaseCost * (1 + totalBuy * (increasePriceEndlessMode / 100f));
            }
        }
        
       
        
        return newPrice;
    }

    private UpgradeCardRunTimeData GetOrCreateRunTimeData(UpgradeCardSO so)
    {
        if (!_runTimeData.TryGetValue(so, out UpgradeCardRunTimeData data))
        {
            data = new UpgradeCardRunTimeData
            {
                CardSo = so,
                TotalBuy = 0,
                CurrentPrice = Mathf.RoundToInt(GetNewPrice(so, _totalAllUpgrades))
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
            data.TotalBuy > 0)
            return false;

        if (upgradeCardSo.upgradeType == UpgradeType.AbilityCardUpgrade)
        {
            //And show base on Level That Skill
            //Example we have lvl. 1 skill, show the Upgrade to next level
            var skillData = SkillCardManager.Instance.GetActiveSkillCardSo(upgradeCardSo.baseSkillCard);
            if (skillData == null) return false;
            
            int skillLevel = skillData.skillLevel + 1;
            return upgradeCardSo.linkedSkillCard != null && SkillCardManager.Instance.OwnedSkillCards.Contains(upgradeCardSo.baseSkillCard) && skillLevel == upgradeCardSo.upgradeValue;
        }
        
        return true;
    }

    private string GetCategoryKey(UpgradeCardSO upgradeCardSo)
    {
        if (upgradeCardSo.upgradeType == UpgradeType.AbilityCard || upgradeCardSo.upgradeType == UpgradeType.AbilityCardUpgrade)
            return upgradeCardSo.linkedSkillCard != null ? $"Ability_{upgradeCardSo.linkedSkillCard.name}" : $"Ability_{upgradeCardSo.upgradeName}";

        return upgradeCardSo.upgradeStatusType.ToString();
    }
    
    public void OnShowRandomUpgradeCard()
    {
        // Show Random Update Card by Day count foe the pool
        if (GameManager.Instance.GameMode == GameMode.Story)
        {
            int dayCount = DayCycleManager.Instance.DayCount;
            var poolData = upgradeCardPools.Find(x => x.days >= dayCount);
            if (poolData == null)
            {
                Debug.LogError($"[{name} - (OnShowRandomUpgradeCard)] No pool found for day {dayCount}!");
                return;
            }

            _selectedPool = poolData;
        }
        
        float accumlate = 0;

        if (_selectedPool == null)
        {
            Debug.LogError($"[{name} - (OnShowRandomUpgradeCard)] No selected pool!");
            return;
        }
        
        foreach (var upgradeData in _selectedPool.upgradeCardDatas)
            accumlate += upgradeData.change;
        
        if (accumlate > 100f)
        {
            Debug.LogError($"[{name} - (OnShowRandomUpgradeCard)] Cannot Show Random Upgrade Card the change is {accumlate}!");
            return;
        }
        
        activeUpgradeCards.Clear();
        var remainingBuckets = _selectedPool.upgradeCardDatas.ToList();
        
        int attempts = 0;
        int maxAttempts = maxActiveRandomCards * 30;
        var categoryUpgradeStatusType = new HashSet<string>();
        
        while (activeUpgradeCards.Count < maxActiveRandomCards && attempts < maxAttempts && remainingBuckets.Count > 0)
        {
            attempts++;

            var picked = PickWeightedCardData(remainingBuckets);
            if (picked == null || picked.listOfUpgradeCardSo.Count == 0)
            {
                remainingBuckets.Remove(picked);
                continue;
            }

            var candidates = picked.listOfUpgradeCardSo
                .Where(so => IsCardAvailable(so)
                             && !activeUpgradeCards.Contains(so)
                             && !categoryUpgradeStatusType.Contains(GetCategoryKey(so)))
                .ToList();
            
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
        {
            var leftOverCandidates = _selectedPool.upgradeCardDatas
                .SelectMany(d => d.listOfUpgradeCardSo)
                .Where(so => IsCardAvailable(so)
                             && !activeUpgradeCards.Contains(so)
                             && !categoryUpgradeStatusType.Contains(GetCategoryKey(so)))
                .Distinct()
                .OrderBy(_ => Random.value)
                .ToList();

            foreach (var upgradeCardData in leftOverCandidates)
            {
                if (activeUpgradeCards.Count >= maxActiveRandomCards)
                    break;

                activeUpgradeCards.Add(upgradeCardData);
            }
        }
        
        if (activeUpgradeCards.Count < maxActiveRandomCards)
            Debug.LogWarning($"[{name} - (OnShowRandomUpgradeCard)] Only found {activeUpgradeCards.Count}/{maxActiveRandomCards} cards after {attempts} attempts. Not enough available cards in pool '{_selectedPool.poolName}'.");
        
        for (int i = 0; i < maxActiveRandomCards; i++)
        {
            if (upgradeCardUi[i] == null)
                continue;
            
            if (i < activeUpgradeCards.Count)
            {
                upgradeCardUi[i].gameObject.SetActive(true);
                upgradeCardUi[i].InitilizeUpgradeCardUI(GetOrCreateRunTimeData(activeUpgradeCards[i]));
            }
        }
    }
    
    public bool OnUpgradeCard(UpgradeCardSO upgradeCardData)
    {
        var data = GetOrCreateRunTimeData(upgradeCardData);
        if (!CurrencyManager.Instance.UseCurrency(data.CurrentPrice))
            return false;
        
        ApplyUpgradeEffect(upgradeCardData);
        
        data.TotalBuy++;
        
        foreach (var kvp in _runTimeData)
            _totalAllUpgrades += kvp.Value.TotalBuy;

        foreach (var kvp in _runTimeData)
        {
            kvp.Value.CurrentPrice = Mathf.RoundToInt(GetNewPrice(kvp.Key, _totalAllUpgrades));
            Debug.Log($"[{kvp.Key.upgradeName}] New Current Price: {kvp.Value.CurrentPrice}");
        }
        
        return true;
    }
}
