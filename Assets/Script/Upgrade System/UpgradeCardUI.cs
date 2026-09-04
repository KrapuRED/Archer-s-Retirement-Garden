using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Tools;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private UpgradeCardSO upgradeCardData;
    [SerializeField] private TMP_Text nameUpgradeCard;
    [SerializeField] private TMP_Text descriptionUpgradeCard;
    [SerializeField] private TMP_Text costUpgradeCard;
    [SerializeField] private Image iconUpgradeCard;

    [Header("Feedbacks")] 
    [SerializeField] private MMFeedbacks insufficientFeedback;
    
    [SerializeField] private Transform starContiner;
    private List<Transform> _starImage = new();

    private void Awake()
    {
        for (int i = 0; i < starContiner.childCount; i++)
        {
            _starImage.Add(starContiner.GetChild(i));
        }
    }

    private void Start()
    {
        if (upgradeCardData == null) return;
        
        nameUpgradeCard.text = upgradeCardData.upgradeName;
        descriptionUpgradeCard.text = upgradeCardData.upgradeDescription;
        costUpgradeCard.text = $"{Mathf.RoundToInt(upgradeCardData.upgradeBaseCost)}";
    }
    
    private void ShowStarByRarity(UpgradeRarity rarity)
    {
        int starCount = (int)rarity + 1; // Star1 -> 1, Star2 -> 2, Star3 -> 3

        for (int i = 0; i < _starImage.Count; i++)
        {
            _starImage[i].gameObject.SetActive(i < starCount);
        }
    }

    public void InitilizeUpgradeCardUI(UpgradeCardRunTimeData upgradeData)
    {
        //Show Star By Rarity
        upgradeCardData = upgradeData.CardSo;
        
        this.name = $"Upgrade Card - {upgradeData.CardSo.upgradeName}";
        ShowStarByRarity(upgradeData.CardSo.rarity);

        if (iconUpgradeCard != null)
            iconUpgradeCard.sprite = upgradeData.CardSo.upgradeIcon;
        
        nameUpgradeCard.text = upgradeData.CardSo.upgradeName;
        descriptionUpgradeCard.text = upgradeData.CardSo.upgradeDescription;
        costUpgradeCard.text = $"{Mathf.RoundToInt(upgradeData.CurrentPrice)} $";
    }

    public void OnClickUgradeCard()
    { 
        if (!UpgradeCardManager.Instance.OnUpgradeCard(upgradeCardData))
        {
            insufficientFeedback?.PlayFeedbacks();
            return;
        }
        
        GameEvents.OnRequestClosePanel.Invoke(PanelType.Upgrade);
    }
}
