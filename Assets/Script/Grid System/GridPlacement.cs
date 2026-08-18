using UnityEngine;
using UnityEngine.InputSystem;

public class GridPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private float distanceRay;
    [SerializeField] private LayerMask groundLayerMask;
    
    [Header("Input Action Settings")]
    [Tooltip("Vector2 action bound to mouse position")]
    [SerializeField] private InputActionReference pointAction;
    [SerializeField] private InputActionReference placeAction;
    [SerializeField] private InputActionReference cancelAction;
    
    [Header("Placement Settings")]
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

    private void Awake()
    {
        mainCam = Camera.main;
    }

    #region Event Configuration
    private void OnEnable()
    {
        pointAction.action.Enable();
        placeAction.action.Enable();
        cancelAction.action.Enable();
        
        pointAction.action.performed += OnPoint;
        placeAction.action.performed += OnPlace;
        cancelAction.action.performed += OnCancel;
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
    }
    
    #endregion

    #region Action Configuration
    private void OnPoint(InputAction.CallbackContext context)
    {
        _screenPos = context.ReadValue<Vector2>();
    }
    
    private void OnPlace(InputAction.CallbackContext context)
    {
        if (_isCanPlaceCurrentCell)
        {
            ConfirmPlacement();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        //CancelPlacement();
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
        Vector3 worldPos = _gridManager.GetFootprintCenter(_anchorCell, objectSize);
        GameObject placed = Instantiate(objectPlacement, worldPos, Quaternion.identity);
        
        _gridManager.PlaceFootPrint(_currentCell, objectSize, placed);
        
        SetPreviewColor(false);
    }

    private void CancelPlacement()
    {
        if (_previewRenderer != null)
        {
            Destroy(_previewRenderer);
        }
        
        objectPlacement = null;
    }

    #endregion
    
    public void BeginPlacement(GameObject prefab)
    {
        CancelPlacement();
        objectPlacement = prefab;
    }
}
