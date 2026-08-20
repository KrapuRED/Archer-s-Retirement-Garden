using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum BoostType
{
    None,
    Attack,
    Critical,
    Heal
}

[System.Serializable]
public class BoostStatusData
{
    public string boostName;
    public float boostAmount;
    public BoostType boostType;
}

[System.Serializable]
public class GardenBoostTrackData
{
    public GardenItemSO gardenItemSO;
    public int stackItemCount;
}

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance { get; private set; }

    [SerializeField] private float maxHealthBoost;
    [SerializeField] private float attackBoost;
    [SerializeField] private float criticalBoost;

    private List<GardenBoostTrackData> _gardenBoostTrackDataList = new();

    public float MaxHealthBoost => maxHealthBoost;
    public float AttackBoost => attackBoost;
    public float CriticalBoost => criticalBoost;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    private void HandlingBoost(GardenItemSO gardenItemSO, int stack)
    {
        if (gardenItemSO.gardenBoostItemDatas == null || gardenItemSO.gardenBoostItemDatas.Count == 0)
            return;

        var trackData = _gardenBoostTrackDataList.Find(x => x.gardenItemSO == gardenItemSO);
        if (trackData == null)
        {
            trackData = new GardenBoostTrackData { gardenItemSO = gardenItemSO, stackItemCount = 0 };
            _gardenBoostTrackDataList.Add(trackData);
        }

        if (trackData.stackItemCount > stack)
        {
            RemoveBoosting(gardenItemSO, stack);
        }
        else
        {
            AddBoosting(gardenItemSO, stack);
        }
        
        trackData.stackItemCount = stack;
    }

    private void AddBoosting(GardenItemSO gardenItemSO, int stackDelta)
    {
        foreach (var boostData in gardenItemSO.gardenBoostItemDatas)
        {
            float amount =  boostData.boostAmount;
            ApplyBoost(boostData.boostType, amount);
        }
    }

    private void RemoveBoosting(GardenItemSO gardenItemSO, int stackDelta)
    {
        foreach (var boostData in gardenItemSO.gardenBoostItemDatas)
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
                criticalBoost += amount;
                break;
            case BoostType.Heal:
                maxHealthBoost += amount;
                break;
        }
    }
}
