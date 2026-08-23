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
    [SerializeField] private GardenItemCardData gardenItemCardData;
    [SerializeField] private GameObject objectPlacement;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    [SerializeField] private GameObject testObject;
    
    private GameObject _previewInstance;
    private Renderer _previewRenderer;
    private Vector2Int _currentCell;
    private Vector2Int _anchorCell;
    private bool _isCanPlaceCurrentCell;
    private Vector2 _screenPos;
    private bool _onConfirmation;

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
        GameEvents.OnActionMapChange.AddListener(OnChangeActionMap);
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

    private void OnChangeActionMap()
    {
        if (_previewInstance != null)
        {
            Destroy(_previewInstance);
            _previewInstance = null;
        }
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
            //Call Confirmation Panel
            _onConfirmation = true;
            GameEvents.OnRequestOpenPanel.Invoke(PanelType.Confirmation);
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
        if (_onConfirmation) return;
        
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
            
        _anchorCell = rawCell - new Vector2Int(gardenItemCardData.gardenItemSO.objectSize.x / 2, gardenItemCardData.gardenItemSO.objectSize.y / 2);
        _currentCell = _anchorCell;
        
        Vector3 worldPos = _gridManager.GetFootprintCenter(_anchorCell, gardenItemCardData.gardenItemSO.objectSize);
        
        _previewInstance.transform.position = worldPos;
        
        _isCanPlaceCurrentCell = _gridManager.CanPlaceFootPrint(_anchorCell, gardenItemCardData.gardenItemSO.objectSize);
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
    
    public void ConfirmPlacement()
    {
        if (objectPlacement == null) return;

        if (CurrencyManager.Instance.UseCurrency(gardenItemCardData.gardenItemSO.gardenItemBasePrice))
        {
            Vector3 worldPos = _gridManager.GetFootprintCenter(_anchorCell, gardenItemCardData.gardenItemSO.objectSize);

            GameObject placed = Instantiate(objectPlacement, worldPos, Quaternion.identity);
        
            _gridManager.PlaceFootPrint(_currentCell, gardenItemCardData.gardenItemSO.objectSize, placed);
            
            var gardenObject = placed.GetComponent<GardenObject>();
            if (gardenObject != null)
            {
                gardenObject.Initialize(gardenItemCardData.gardenItemSO, _anchorCell);
            }
            else
            {
                Debug.LogWarning($"[{name} (ConfirmPlacement)] {placed.name} has no GardenObject component - it won't be sellable.");
            }
            
            SetPreviewColor(false);
        }
        
        CancelPlacement();
    }

    public void CancelPlacement()
    {
        if (_previewInstance != null)
        {
            Destroy(_previewInstance);
        }

        _onConfirmation = false;
        _previewInstance = null;
        gardenItemCardData = null;
        
        InputManager.Instance.PopInputActionMap();
        objectPlacement = null;
    }

    #endregion
    
    private void BeginPlacement(GardenItemCardData gardenItemCardData)
    {
        CancelPlacement();
        
        InputManager.Instance.SwitchInputMap(actionMapName);
        this.gardenItemCardData  = gardenItemCardData;
        objectPlacement = gardenItemCardData.gardenItemSO.objectPlacement;
    }
}
