using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GardenItemCardData
{
    public string gardenItemName;
    public GardenItemSO gardenItemSO;
    public GardenItemCard gardenItemCard;
    public int currentPrice;
    public int stackItemCount;
}

public class GardenItemCardHolder : MonoBehaviour
{
    [SerializeField] private GardenItemCard prefabGardenItemCard;
    [SerializeField] private Transform cardHolderTransform;
    [SerializeField] private List<GardenItemSO> gardenItemDatas = new();
    [SerializeField] private List<GardenItemCardData> gardenItemCardDataList = new();
    
    private bool _isInitialized;
    
    #region Event Configuration

    private void OnEnable()
    {
        GameEvents.OnChangeStackGardenObject.AddListener(ChangePriceCard);
    }

    private void OnDisable()
    {
        GameEvents.OnChangeStackGardenObject.RemoveListener(ChangePriceCard);
    }
   
    #endregion
    
    public void Init()
    {
        gardenItemCardDataList.Clear();

        foreach (var gardenItemData in gardenItemDatas)
        {
            GardenItemCardData cardData = new GardenItemCardData
            {
                gardenItemName =  gardenItemData.gardenItemName,
                gardenItemSO = gardenItemData,
                currentPrice = gardenItemData.gardenItemBasePrice
            };

            if (!gardenItemDatas.Exists(x => x.gardenItemName == cardData.gardenItemName))
            {
                Debug.LogWarning($"[{name} (Init)] there are already card with name {cardData.gardenItemName}");
                continue;
            }
            
            var newCard = Instantiate(prefabGardenItemCard, cardHolderTransform);
            if (newCard == null)
            {
                Destroy(newCard);
                return;
            }
            
            newCard.name = gardenItemData.gardenItemName;
            newCard.Init(cardData);
            cardData.gardenItemCard = newCard;
            
            gardenItemCardDataList.Add(cardData);
        }
        
        _isInitialized = true;
    }

    private void IncreasePriceCard(GardenItemCardData cardData)
    {
        float increaseAmount = cardData.gardenItemSO.gardenItemBasePrice * (cardData.gardenItemSO.priceIncrease / 100f);
        int newPrice = cardData.currentPrice + Mathf.RoundToInt(increaseAmount);

        cardData.currentPrice = newPrice;
        cardData.gardenItemCard.UpdatePrice(newPrice);
    }

    private void DecreasePriceCard(GardenItemCardData cardData)
    {
        float increaseAmount = cardData.gardenItemSO.gardenItemBasePrice * (cardData.gardenItemSO.priceIncrease / 100f);
        int newPrice = cardData.currentPrice - Mathf.RoundToInt(increaseAmount);

        cardData.currentPrice = newPrice;
        cardData.gardenItemCard.UpdatePrice(newPrice);
    }
    
    private void ChangePriceCard(GardenItemSO gardenItemSO, int stack)
    {
        if (!_isInitialized) return;
        
        // 1) Find gardenItemSO in the gardenItemCardDataList
        var cardData = gardenItemCardDataList.Find(x => x.gardenItemSO == gardenItemSO);

        if (cardData.stackItemCount > stack)
        {
            DecreasePriceCard(cardData);
        }
        else
        {
            IncreasePriceCard(cardData);
        }
        
        cardData.stackItemCount = stack;
    }
}
