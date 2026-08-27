using TMPro;
using UnityEngine;
using System.Text;

public class GardenInformationCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text gardenItemName;
    [SerializeField] private TMP_Text gardenItemPrice;
    [SerializeField] private TMP_Text gardenItemBoosts;

    private readonly StringBuilder _builder = new();
    
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

    private void SetGardenInformationCard(GardenItemCardData gardenItemData)
    {
        gardenItemName.text  = gardenItemData.gardenItemName;
        gardenItemPrice.text = $"{gardenItemData.currentPrice} $";

        _builder.Clear();
        
        foreach (var boosData in gardenItemData.gardenItemSO.gardenBoostItemDatas)
        {
            _builder.Append(boosData.boostName);
            _builder.Append('\n');
        }
        
        gardenItemBoosts.text = _builder.ToString();
    }

    private void ClearGardenInformationCard()
    {
        gardenItemName.text = "";
        gardenItemPrice.text = "";
        gardenItemBoosts.text = "";
    }
}
