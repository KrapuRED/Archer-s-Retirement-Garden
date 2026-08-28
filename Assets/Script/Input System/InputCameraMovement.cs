using UnityEngine;
using UnityEngine.InputSystem;

public class InputCameraMovement : MonoBehaviour, IPauseable
{
    [Header("Input Action Configuration")]
    [SerializeField] private InputActionReference cameraMovementAction;

    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float speedCamMovement;
    [SerializeField] private float smoothTime;
    
    private Vector2 _input;
    private Vector3 _currentVelocity;
    public bool IsPaused  { get; set; }
    
    private void OnEnable()
    {
        cameraMovementAction.action.Enable();
        
        cameraMovementAction.action.performed   += OnMoveCamera;
        cameraMovementAction.action.canceled    += OnMoveCamera;
        
        GameEvents.OnPauseGame.AddListener(Pause);
        GameEvents.OnResumeGame.AddListener(Resume);
    }

    private void OnDisable()
    {
        cameraMovementAction.action.performed   -= OnMoveCamera;
        cameraMovementAction.action.canceled    -= OnMoveCamera;
        
        GameEvents.OnPauseGame.RemoveListener(Pause);
        GameEvents.OnResumeGame.RemoveListener(Resume);
    }

    private void OnMoveCamera(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameActive) return;
            
        if (IsPaused) return;
        
        if (_input.sqrMagnitude < 0.0001f)
        {
            _currentVelocity = Vector3.zero;
            return;
        }
        
        Vector3 movementDir = new Vector3(_input.x, 0, _input.y);
        Vector3 movement = movementDir * (speedCamMovement * Time.deltaTime);
        Vector3 targetPos = cameraPivot.position + movement;
        
        cameraPivot.position = Vector3.SmoothDamp(cameraPivot.position, targetPos, ref _currentVelocity, smoothTime);
    }

    #region Interface

    public void Pause()
    {
        IsPaused = true;
        _currentVelocity = Vector3.zero; 
    }

    public void Resume()
    {
        IsPaused = false;
    }

    #endregion
}
