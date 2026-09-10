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
    
    [Header("Input Skill Action Configuration")]
    [SerializeField] private InputActionReference skillAction1;
    [SerializeField] private InputActionReference skillAction2;
    [SerializeField] private InputActionReference skillAction3;
    [SerializeField] private InputActionReference skillAction4;
   
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
    private SkillCardDataRunTime _currentPreviewedSkill;
    private float _fixedY;
    
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
        
        skillAction1.action.Enable();
        skillAction2.action.Enable();
        skillAction3.action.Enable();
        skillAction4.action.Enable();
        
        mousePositionAction.action.performed += OnPositionMouse;
        attackAction.action.performed   += OnClickAttack;
        cancelAction.action.performed   += OnCancelAction;
        
        skillAction1.action.performed += OnSelectSkill1;
        skillAction2.action.performed += OnSelectSkill2;
        skillAction3.action.performed += OnSelectSkill3;
        skillAction4.action.performed += OnSelectSkill4;
        
        GameEvents.OnActionMapChange.AddListener(OnChangeActionMap);
    }

    private void OnDisable()
    {
        mousePositionAction.action.performed -= OnPositionMouse;
        attackAction.action.performed   -= OnClickAttack;
        cancelAction.action.performed   -= OnCancelAction;
        
        skillAction1.action.performed -= OnSelectSkill1;
        skillAction2.action.performed -= OnSelectSkill2;
        skillAction3.action.performed -= OnSelectSkill3;
        skillAction4.action.performed -= OnSelectSkill4;
        
        GameEvents.OnActionMapChange.RemoveListener(OnChangeActionMap);
        
    }

    private void OnPositionMouse(InputAction.CallbackContext ctx)
    {
        _screenPosition = ctx.ReadValue<Vector2>();
    }

    private void OnSelectSkill1(InputAction.CallbackContext ctx) => TrySelectSkillByKey(0);
    private void OnSelectSkill2(InputAction.CallbackContext ctx) => TrySelectSkillByKey(1);
    private void OnSelectSkill3(InputAction.CallbackContext ctx) => TrySelectSkillByKey(2);
    private void OnSelectSkill4(InputAction.CallbackContext ctx) => TrySelectSkillByKey(3);

    private void TrySelectSkillByKey(int indexSkill)
    {
        if (!InputManager.Instance.IsInputMapActive(actionMapName))
            return;

        Debug.LogWarning($"[{name} - TrySelectSkillByKey] TrySelectSkillByKey {indexSkill}");
        SkillCardManager.Instance.SelectSkillCardByKey(indexSkill);
    }
    
    private void OnClickAttack(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; 
        }
        
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
        
        SkillCardDataRunTime selectedSkill = SkillCardManager.Instance.SelectedSkillCard;
        
        if (_previewInstance != null && _currentPreviewedSkill != selectedSkill)
        {
            Destroy(_previewInstance);
            _previewInstance = null;
        }
        
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
        
        _currentPreviewedSkill = skillCardDataRunTime;
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
