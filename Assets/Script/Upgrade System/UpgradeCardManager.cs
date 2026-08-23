using System.Collections.Generic;
using UnityEngine;

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
    public List<UpgradeCardData> UpgradeCardDatas = new();
}

public class UpgradeCardManager : MonoBehaviour
{
    public static UpgradeCardManager Instance { get; private set; }

    [SerializeField] private Transform upgradeCardContainer;
    [SerializeField] private List<UpgradeCardPool> upgradeCardPools = new();
    
    private bool _isInitialized;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return; 
        }

        Instance = this;
    }

    private void Initialize()
    {
        _isInitialized = true;
    }
}
