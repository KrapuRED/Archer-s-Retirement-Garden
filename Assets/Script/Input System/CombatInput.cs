using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CombatInput : MonoBehaviour
{
    [Header("Input Action Configuration")]
    [SerializeField] private string actionMapName;
    [SerializeField] private InputActionReference mousePositionAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference cancelAction;
    
    [Header("Preview Target Configuration")]
    [SerializeField] private Vector2Int previewTargetSize;
    [SerializeField] private float distanceRay;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform previewTargetContainer;
    
    private GameObject _previewInstance;
    private Renderer _previewRenderer;
    private Vector2 _screenPosition;
    private Vector2Int _currentCell;
    private Vector2Int _anchorCell;

    private bool _isInsideUI;
    [SerializeField] private float _fixedY;
    
    private GridManager _gridManager;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        mousePositionAction.action.Enable();
        attackAction.action.Enable();
        cancelAction.action.Enable();

        mousePositionAction.action.performed += OnPositionMouse;
        attackAction.action.performed   += OnClickAttack;
        cancelAction.action.performed   += OnCancelAction;
        
        GameEvents.OnActionMapChange.AddListener(OnChangeActionMap);
    }

    private void OnDisable()
    {
        mousePositionAction.action.performed -= OnPositionMouse;
        attackAction.action.performed   -= OnClickAttack;
        cancelAction.action.performed   -= OnCancelAction;
        
        GameEvents.OnActionMapChange.RemoveListener(OnChangeActionMap);
        
    }

    private void OnPositionMouse(InputAction.CallbackContext ctx)
    {
        _screenPosition = ctx.ReadValue<Vector2>();
    }

    private void OnClickAttack(InputAction.CallbackContext ctx)
    {
        if (_isInsideUI) return;
        
        if (!InputManager.Instance.IsInputMapActive(actionMapName))
            return;

        SkillCardDataRunTime skillCardDataRunTime = SkillCardManager.Instance.SelectedSkillCard;
        
        if (skillCardDataRunTime == null || skillCardDataRunTime.skillCardSo == null)
        {
            Debug.LogWarning($"[{name} - (OnClickAttack)] SkillCardData is null!");
            return;
        }
        
        SkillCardManager.Instance.UsingSkillCard(_previewInstance);
        
        _previewInstance = null;
    }

    private void OnCancelAction(InputAction.CallbackContext ctx)
    {
        if (!InputManager.Instance.IsInputMapActive(actionMapName))
            return;
        
        Destroy(_previewInstance);
        SkillCardManager.Instance.CancelSkillCard();
    }

    private void FixedUpdate()
    {
        if (!InputManager.Instance.IsInputMapActive(actionMapName))
            return;
        
        if (_previewInstance == null)
            SpawnPreviewTarget();
        
        UpdatePreviewTarget();
    }

    private void SpawnPreviewTarget()
    {
        SkillCardDataRunTime skillCardDataRunTime = SkillCardManager.Instance.SelectedSkillCard;

        if (skillCardDataRunTime == null || skillCardDataRunTime.skillCardSo == null)
        {
            return;
        }
        
        _previewInstance = Instantiate(skillCardDataRunTime.skillCardSo.prefabSkillTargeting, previewTargetContainer);
        _previewRenderer = _previewInstance.GetComponent<Renderer>();
        
        _fixedY = _previewInstance.transform.position.y;
        
        foreach (var col in _previewInstance.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    private void OnChangeActionMap()
    {
        foreach (Transform prevPreview in previewTargetContainer)
        {
            Destroy(prevPreview.gameObject);
        }
        
        Destroy(_previewInstance);
    }
    
    private void UpdatePreviewTarget()
    {
        if (_previewInstance == null)
            return;
        
        if (_gridManager == null)
            _gridManager = GridManager.Instance;
        
        Ray ray = _camera.ScreenPointToRay(_screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, distanceRay, groundLayerMask)) return;

        Vector3 clamped = _gridManager.ClampToGridBounds(hit.point);
        clamped.y = _fixedY;
        _previewInstance.transform.position = clamped;

        _anchorCell = _gridManager.WorldToGrid(clamped) - - new Vector2Int(previewTargetSize.x / 2, previewTargetSize.y / 2);
        _currentCell = _anchorCell;
    }
}
