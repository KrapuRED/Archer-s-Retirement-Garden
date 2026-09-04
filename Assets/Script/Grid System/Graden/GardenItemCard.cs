using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GardenItemCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   [Header("Input Action Settings")]
   [SerializeField] private string actionMapName;
   [SerializeField] private InputActionReference holdCardAction;
   
   [SerializeField] private Image gardenItemImage;
   [SerializeField] private TMP_Text gardenItemCostText;

   [SerializeField] private GardenItemCardData gardenItemCardData;

   private bool _isHovering;
   
   #region Event Configuration

   private void OnEnable()
   {
      holdCardAction.action.Enable();

      holdCardAction.action.performed += OnHoldButton;
   }

   private void OnDisable()
   {
      holdCardAction.action.performed -= OnHoldButton;
   }
   
   #endregion
   
   #region Pointer Hover Tracking

   public void OnPointerEnter(PointerEventData eventData)
   {
      _isHovering = true;
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      _isHovering = false;
   }

   #endregion

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
      GameEvents.OnShowDetailGardenItem.Invoke(gardenItemCardData);
   }
   
   private void OnHoldButton(InputAction.CallbackContext context)
   {
      if (!InputManager.Instance.IsInputMapActive(actionMapName))
         return;
      
      if (!_isHovering) return;
      
      Debug.Log($"[{name} (OnHoldButton)] gardenItem: {gardenItemCardData.gardenItemSO.gardenItemName}");
      
      GameEvents.OnCarryObject.Invoke(gardenItemCardData);
   }

}
