using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GardenItemCard : MonoBehaviour
{
   [Header("Input Action Settings")]
   [SerializeField] private string actionMapName;
   [SerializeField] private InputActionReference holdCardAction;
   
   [SerializeField] private Image gardenItemImage;
   [SerializeField] private TMP_Text gardenItemCostText;

   [SerializeField] private GardenItemCardData gardenItemCardData;

   private bool _isHovering;
   
   public void Init(GardenItemCardData gardenItemCardData)
   {
      this.gardenItemCardData = gardenItemCardData;

      gardenItemImage.sprite = gardenItemCardData.gardenItemSO.gardenItemImage;
      gardenItemCostText.text = $"{gardenItemCardData.currentPrice}";
   }

   public void UpdatePrice(int newPrice)
   {
      gardenItemCardData.currentPrice = newPrice;
      gardenItemCostText.text = $"{gardenItemCardData.currentPrice}";
   }
   
   public void OnClickButton()
   {
      if (!InputManager.Instance.IsInputMapActive(actionMapName))
         return;
      
      if (!_isHovering) return;
      
      GameEvents.OnCarryObject.Invoke(gardenItemCardData);
   }

   public void SelecteCard()
   {
      if (_isHovering) return;
      
      _isHovering = true;
      GameEvents.OnShowDetailGardenItem.Invoke(gardenItemCardData);  
   }
   
   public void UnSelecteCard()
   {
      if (!_isHovering) return;
      
      _isHovering = false;
      GameEvents.OnHideDetailGardenItem.Invoke();
   }
}
