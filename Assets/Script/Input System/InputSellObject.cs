using UnityEngine;
using UnityEngine.InputSystem;

public class InputSellObject : MonoBehaviour
{
    [Header("Input Action Configuration")]
    [SerializeField] private string actionMapName;
    [SerializeField] private InputActionReference sellPositionAction;
    [SerializeField] private InputActionReference sellClickAction;

    [SerializeField] private float distanceRay;
    [SerializeField] private LayerMask gardenLayerMask;
    
    private Camera _mainCamera;
    private Vector2 _screenPos;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        sellClickAction.action.Enable();
        sellPositionAction.action.Enable();
        
        sellPositionAction.action.performed += SellPosition;
        sellClickAction.action.performed    += SellObject;
    }

    private void OnDisable()
    {
        sellPositionAction.action.performed -= SellPosition;
        sellClickAction.action.performed    -= SellObject;
    }

    private void SellPosition(InputAction.CallbackContext ctx) => _screenPos = ctx.ReadValue<Vector2>();
    
    private void SellObject(InputAction.CallbackContext context)
    {
        if (!InputManager.Instance.IsInputMapActive(actionMapName)) return;
        
        Ray ray = _mainCamera.ScreenPointToRay(_screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit, distanceRay, gardenLayerMask)) return;

        var obj = hit.collider.GetComponent<GardenObject>();
        if (obj == null) return;

        obj.SellGardenObject();
    }
}
