using UnityEngine;
using UnityEngine.InputSystem;

public class GridPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private float distanceRay;
    [SerializeField] private LayerMask groundLayerMask;
    
    [Header("Input Action Settings")]
    [SerializeField] private string actionMapName;
    [Tooltip("Vector2 action bound to mouse position")]
    [SerializeField] private InputActionReference pointAction;
    [SerializeField] private InputActionReference placeAction;
    [SerializeField] private InputActionReference cancelAction;

    [Header("Placement Settings")] 
    [SerializeField] private GardenItemSO gardenItemData;
    [SerializeField] private GameObject objectPlacement;
    [Tooltip("How many cells this object occupies, e.g. (2,2) = a 2x2 footprint (4 cells).")]
    [SerializeField] private Vector2Int objectSize = Vector2Int.one;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    [SerializeField] private GameObject testObject;
    
    private GameObject _previewInstance;
    private Renderer _previewRenderer;
    private Vector2Int _currentCell;
    private Vector2Int _anchorCell;
    private bool _isCanPlaceCurrentCell;
    private Vector2 _screenPos;

    private GridManager _gridManager;
    private InputManager _inputManager;

    private void Awake()
    {
        mainCam = Camera.main;
        
    }

    #region Event Configuration
    private void OnEnable()
    {
        _inputManager = InputManager.Instance;
        
        pointAction.action.Enable();
        placeAction.action.Enable();
        cancelAction.action.Enable();
        
        pointAction.action.performed += OnPoint;
        placeAction.action.performed += OnPlace;
        cancelAction.action.performed += OnCancel;
        
        GameEvents.OnCarryObject.AddListener(BeginPlacement);
    }

    private void OnDisable()
    {
        OnUnregister();
    }

    private void OnUnregister()
    {
        pointAction.action.performed  -= OnPoint;
        placeAction.action.performed  -= OnPlace;
        cancelAction.action.performed -= OnCancel;
        
        GameEvents.OnCarryObject.RemoveListener(BeginPlacement);
    }
    
    #endregion

    #region Action Configuration
    private void OnPoint(InputAction.CallbackContext context)
    {
        if (_inputManager == null) 
            _inputManager = InputManager.Instance;
        
        if (!_inputManager.IsInputMapActive(actionMapName)) return;
        
        _screenPos = context.ReadValue<Vector2>();
    }
    
    private void OnPlace(InputAction.CallbackContext context)
    {
        if (_inputManager == null) 
            _inputManager = InputManager.Instance;   
        
        if (!_inputManager.IsInputMapActive(actionMapName)) return;
        
        if (_isCanPlaceCurrentCell)
        {
            ConfirmPlacement();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (_inputManager == null) 
            _inputManager = InputManager.Instance;
        
        if (!_inputManager.IsInputMapActive(actionMapName)) return;
        
        CancelPlacement();
        InputManager.Instance.PopInputActionMap();
    }

    #endregion

    private void FixedUpdate()
    {
        if (objectPlacement == null) return;

        if (_previewInstance == null)
        {
            SpawnPreview();
        }

        UpdatePreviewPosition();
    }

    #region Main Grid Placement Functions

    private void SpawnPreview()
    {
        _previewInstance = Instantiate(objectPlacement);
        _previewRenderer = _previewInstance.GetComponent<Renderer>();
        
        foreach (var col in _previewInstance.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    private void UpdatePreviewPosition()
    {
        if (_gridManager == null) 
            _gridManager = GridManager.Instance;
        
        Ray ray = mainCam.ScreenPointToRay(_screenPos);
        
        if (!Physics.Raycast(ray, out RaycastHit hit, distanceRay, groundLayerMask)) return;

        Vector2Int rawCell = _gridManager.WorldToGrid(hit.point);
            
        _anchorCell = rawCell - new Vector2Int(objectSize.x / 2, objectSize.y / 2);
        _currentCell = _anchorCell;
        
        Vector3 worldPos = _gridManager.GridToWorld(_currentCell);
        
        _previewInstance.transform.position = worldPos;
        
        _isCanPlaceCurrentCell = _gridManager.CanPlaceFootPrint(_anchorCell, objectSize);
        SetPreviewColor(_isCanPlaceCurrentCell);
    }

    private void SetPreviewColor(bool valid)
    {
        if (_previewRenderer == null)
        {
            Debug.LogError($"[{name} (SetPreviewColor)] Preview Renderer is null!");
            return;
        }
        
        _previewRenderer.material = valid ? validMaterial : invalidMaterial;
    }
    
    private void ConfirmPlacement()
    {
        if (objectPlacement == null) return;

        if (CurrencyManager.Instance.UseCurrency(gardenItemData.gardenItemCost))
        {
            Vector3 worldPos = _gridManager.GetFootprintCenter(_anchorCell, objectSize);
            GameObject placed = Instantiate(objectPlacement, worldPos, Quaternion.identity);
        
            _gridManager.PlaceFootPrint(_currentCell, objectSize, placed);
            SetPreviewColor(false);
        }
        
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        if (_previewInstance != null)
        {
            Destroy(_previewInstance);
        }
        
        _previewInstance = null;
        gardenItemData = null;
        
        InputManager.Instance.PopInputActionMap();
        objectPlacement = null;
    }

    #endregion
    
    private void BeginPlacement(GardenItemSO gardenItemSo)
    {
        CancelPlacement();
        
        InputManager.Instance.SwitchInputMap(actionMapName);
        gardenItemData  = gardenItemSo;
        objectPlacement = gardenItemSo.objectPlacement;
    }
}
