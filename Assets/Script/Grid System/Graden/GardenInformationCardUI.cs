using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using MoreMountains.Feedbacks;

public class GardenInformationCardUI : MonoBehaviour
{
    [SerializeField] private Image gardenItemImage;
    [SerializeField] private TMP_Text gardenItemName;
    [SerializeField] private TMP_Text gardenItemPrice;
    [SerializeField] private TMP_Text gardenItemBoosts;

    [Header("Visusal Effect / Animation")] 
    [SerializeField] private MMFeedbacks showCard;
    [SerializeField] private MMFeedbacks hideCard;
    
    private readonly StringBuilder _builder = new();
    private bool _isShowing;
    
    #region Event Configuration
    private void OnEnable()
    {
        GameEvents.OnShowDetailGardenItem.AddListener(SetGardenInformationCard);
        GameEvents.OnHideDetailGardenItem.AddListener(ClearGardenInformationCard);
    }

    private void OnDisable()
    {
        OnRemoveListener();
    }

    private void OnDestroy()
    {
        OnRemoveListener();
    }

    private void OnRemoveListener()
    {
        GameEvents.OnShowDetailGardenItem.RemoveListener(SetGardenInformationCard);
        GameEvents.OnHideDetailGardenItem.RemoveListener(ClearGardenInformationCard);
    }
    
    #endregion

    private void Start()
    {
        hideCard?.PlayFeedbacks();
    }

    private void SetGardenInformationCard(GardenItemCardData gardenItemData)
    {
        gardenItemName.text  = gardenItemData.gardenItemName;
        gardenItemPrice.text = $"{gardenItemData.currentPrice} $";
        gardenItemImage.sprite = gardenItemData.gardenItemSO.gardenItemImage;
            
        _builder.Clear();
        
        foreach (var boosData in gardenItemData.gardenItemSO.gardenBoostItemDatas)
        {
            _builder.Append(boosData.boostName);
            _builder.Append('\n');
        }
        
        gardenItemBoosts.text = _builder.ToString();

        if (!_isShowing)
        {
            _isShowing = true;
            showCard?.PlayFeedbacks();
        }
    }

    public void ClearGardenInformationCard()
    {
        gardenItemName.text = "";
        gardenItemPrice.text = "";
        gardenItemBoosts.text = "";
        
        _isShowing = false;
        hideCard?.PlayFeedbacks();
    }
}
