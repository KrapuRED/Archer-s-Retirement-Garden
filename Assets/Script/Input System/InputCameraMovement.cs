using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputCameraMovement : MonoBehaviour, IPauseable
{
    [SerializeField] private Collider borderCam;
    
    [Header("Input Action Configuration")]
    [SerializeField] private InputActionReference cameraMovementAction;
    [SerializeField] private InputActionReference cameraRotationAction;

    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float speedCamMovement;
    [SerializeField] private float smoothTime;
    
    [Header("Camera Angel Configuration")]
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private List<GameObject> cameraAngels = new ();
    private int _cameraAngelIndex;
    
    private Vector2 _input;
    private Vector3 _currentVelocity;
    public bool IsPaused  { get; set; }

    private void Awake()
    {
        foreach (Transform cameraAngel in cameraContainer)
        {
            cameraAngels.Add(cameraAngel.gameObject);
        }
    }

    private void OnEnable()
    {
        cameraMovementAction.action.Enable();
        cameraRotationAction.action.Enable();
        
        cameraMovementAction.action.performed   += OnMoveCamera;
        cameraMovementAction.action.canceled    += OnMoveCamera;
        cameraRotationAction.action.performed   += OnRotateCamera;
        cameraRotationAction.action.canceled    += OnRotateCamera;
        
        GameEvents.OnPauseGame.AddListener(Pause);
        GameEvents.OnResumeGame.AddListener(Resume);
    }

    private void OnDisable()
    {
        cameraMovementAction.action.performed   -= OnMoveCamera;
        cameraMovementAction.action.canceled    -= OnMoveCamera;
        
        cameraRotationAction.action.performed   -= OnRotateCamera;
        cameraRotationAction.action.canceled    -= OnRotateCamera;
        
        GameEvents.OnPauseGame.RemoveListener(Pause);
        GameEvents.OnResumeGame.RemoveListener(Resume);
    }

    private void OnMoveCamera(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    private void OnRotateCamera(InputAction.CallbackContext context)
    {
        cameraAngels[_cameraAngelIndex].SetActive(false);
        
        _cameraAngelIndex++;
        if (_cameraAngelIndex >= cameraAngels.Count) 
            _cameraAngelIndex = 0;
        
        var cameraAngle = cameraAngels[_cameraAngelIndex];
        cameraAngle.SetActive(true);
        
        Debug.Log($"[{name} - OnRotateCamera] Rotate Camera to {cameraAngle.name}");
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameActive) return;
            
        if (DialogueManager.Instance.IsDialogueRunning) return;
        
        if (IsPaused) return;
        
        if (_input.sqrMagnitude < 0.0001f)
        {
            _currentVelocity = Vector3.zero;
            return;
        }
        
        Vector3 movementDir = new Vector3(_input.x, 0, _input.y);
        Vector3 movement = movementDir * (speedCamMovement * Time.deltaTime);
        Vector3 targetPos = ClampToBorder(cameraPivot.position)  + movement;
        
        cameraPivot.position = Vector3.SmoothDamp(cameraPivot.position, targetPos, ref _currentVelocity, smoothTime);
    }

        
    private Vector3 ClampToBorder(Vector3 position)
    {
        if (borderCam == null)
            return position;

        Bounds bounds = borderCam.bounds;

        position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);
        // y left untouched — you're dragging on a horizontal plane, not clamping height

        return position;
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
