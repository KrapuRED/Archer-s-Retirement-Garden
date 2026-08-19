using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GardenItemCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   [Header("Input Action Settings")]
   [SerializeField] private string actionMapName;
   [SerializeField] private InputActionReference holdCardAction;
   
   [SerializeField] private GardenItemSO gardenItemSo;

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
   
   public void OnClickButton()
   {
      GameEvents.OnShowDetailGardenItem.Invoke(gardenItemSo);
   }
   
   private void OnHoldButton(InputAction.CallbackContext context)
   {
      if (!InputManager.Instance.IsInputMapActive(actionMapName))
         return;
      
      if (!_isHovering) return;
      
      Debug.Log($"[{name} (OnHoldButton)] gardenItem: {gardenItemSo.gardenItemName}");
      GameEvents.OnCarryObject.Invoke(gardenItemSo.objectPlacement);
   }

}
