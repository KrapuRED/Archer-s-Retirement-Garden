using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInput : MonoBehaviour
{
    [Header("Input Action Configuration")]
    [SerializeField] private string actionMapName;
    [SerializeField] private InputActionReference mousePositionAction;
    [SerializeField] private InputActionReference basicAttackAction;
    
    [SerializeField] private GameObject previewTarget;
    [SerializeField] private Vector2Int previewTargetSize;
    [SerializeField] private float distanceRay;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float cooldownBasic;
    
    private GameObject _previewInstance;
    private Renderer _previewRenderer;
    private Vector2 _screenPosition;
    private Vector2Int _currentCell;
    private Vector2Int _anchorCell;

    [SerializeField] private bool isReady = true;
    private GridManager _gridManager;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        mousePositionAction.action.Enable();
        basicAttackAction.action.Enable();

        mousePositionAction.action.performed += OnPositionMouse;
        basicAttackAction.action.performed   += OnClickBasicAttack;
    }

    private void OnDisable()
    {
        mousePositionAction.action.performed -= OnPositionMouse;
        basicAttackAction.action.performed   -= OnClickBasicAttack;
    }

    private void OnPositionMouse(InputAction.CallbackContext ctx)
    {
        if (!InputManager.Instance.IsInputMapActive(actionMapName))
            return;
        
        _screenPosition = ctx.ReadValue<Vector2>();
    }

    private void OnClickBasicAttack(InputAction.CallbackContext ctx)
    {
        if (!InputManager.Instance.IsInputMapActive(actionMapName))
            return;
        
        Debug.Log("Click Basic Attack");
        //Call Arrow out
        
        
        isReady = false;
        StartCoroutine(Cooldown());
    }

    private void FixedUpdate()
    {
        if (!InputManager.Instance.IsInputMapActive(actionMapName))
            return;
        
        if(!isReady) return;
        
        if (_previewInstance == null)
            SpawnPreviewTarget();
        
        UpdatePreviewTarget();
    }

    private void SpawnPreviewTarget()
    {
        _previewInstance = Instantiate(previewTarget);
        _previewRenderer = _previewInstance.GetComponent<Renderer>();
        
        foreach (var col in _previewInstance.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }
    
    private void UpdatePreviewTarget()
    {
        if (_gridManager == null)
            _gridManager = GridManager.Instance;
        Ray ray = _camera.ScreenPointToRay(_screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, distanceRay, groundLayerMask)) return;

        Vector2Int rawCell = _gridManager.WorldToGrid(hit.point);
            
        _anchorCell = rawCell - new Vector2Int(previewTargetSize.x / 2, previewTargetSize.y / 2);
        _currentCell = _anchorCell;
        
        Vector3 worldPos = _gridManager.GetFootprintCenter(_anchorCell, previewTargetSize);
        _previewInstance.transform.position = worldPos;
    }
    
    private IEnumerator Cooldown()
    {
        Destroy(_previewInstance);
        _previewInstance = null;
        
        yield return new WaitForSeconds(cooldownBasic);
        isReady = true;
    }
}
