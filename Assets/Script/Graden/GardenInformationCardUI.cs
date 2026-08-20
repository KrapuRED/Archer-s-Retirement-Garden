using TMPro;
using UnityEngine;

public class GardenInformationCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text gardenItemName;
    [SerializeField] private TMP_Text gardenItemPrice;
    
    #region Event Configuration

    private void OnEnable()
    {
        GameEvents.OnShowDetailGardenItem.AddListener(SetGardenInformationCard);
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
        
    }
    
    #endregion

    private void SetGardenInformationCard(GardenItemSO gardenItemData)
    {
        gardenItemName.text  = gardenItemData.gardenItemName;
        gardenItemPrice.text = $"{gardenItemData.gardenItemBasePrice} $";
    }
}
