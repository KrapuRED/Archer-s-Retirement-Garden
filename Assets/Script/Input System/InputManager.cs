
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public enum CursorType
{
    Default,
    Sell,
    Basic,
    Ability
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input Manager Configuration")]
    [SerializeField] private bool isCursorConfined;
    [SerializeField] private InputActionAsset gamePlayInput;
    [SerializeField] private string defaultActionMap;
    [SerializeField] private string currentActionMapName;
    [SerializeField] private InputActionMap currentActionMap;
    
    [Header("Cursor Configuration")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D sellCursor;
    [SerializeField] private Texture2D basicAttackCursor;
    [SerializeField] private Texture2D abilityActiveCursor;
    
    private readonly Stack<string> _overlayStack = new(); 
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SwitchInputMap(defaultActionMap);
        ChangeCursorTexture(CursorType.Default);
    }

    #region Main Switch Input Manager
    private InputActionMap GetInputActionMap(string actionMapName)
    {
        if (gamePlayInput == null)
        {
            Debug.LogWarning($"[{name} - GetInputMap] Game Play Input is null!");
            return null;
        }
        
        var actionMap = gamePlayInput.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogWarning($"[{name} - GetInputMap] ActionMap not found: {actionMapName}");
            return null;
        }
        
        return actionMap;
    }
    
    private void ExecuteSwitchActionMap(string actionMapName)
    {
        var actionMap = GetInputActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogWarning($"[{name} - ExecuteSwitchActionMap] ActionMap not found: {actionMapName}");
            return;
        }

        if (_overlayStack.Contains(actionMapName))
        {
            Debug.LogWarning($"[{name} - ExecuteSwitchActionMap] '{actionMapName}' is already active in the overlay stack.");

            if (DayCycleManager.Instance.DayCycleType == DayCycleType.Night)
                ChangeCursorTexture(CursorType.Basic);
            else
                ChangeCursorTexture(CursorType.Default);
            
            return;
        }
        
        if (_overlayStack.Count > 0)
            GetInputActionMap(_overlayStack.Peek())?.Disable();
        
        actionMap.Enable();
        _overlayStack.Push(actionMapName);
        currentActionMapName = actionMap.name;
        currentActionMap = actionMap;
        
        GameEvents.OnActionMapChange.Invoke();
    }
    #endregion
    
    public void SwitchInputMap(string actionMapName)
    {
        if (DialogueManager.Instance.IsDialogueRunning) return;
        
        ExecuteSwitchActionMap(actionMapName);
    }
    
    public void PopInputActionMap()
    {
        if (_overlayStack.Count == 0) return;
        
        if (_overlayStack.Count < 0)
        {
            if (currentActionMapName != defaultActionMap)
                ExecuteSwitchActionMap(defaultActionMap);
            return;
        }
        
        string removeActionMap = _overlayStack.Pop();
        GetInputActionMap(removeActionMap)?.Disable();
        
        string nextActionMap = _overlayStack.Count > 0 ? _overlayStack.Peek() : defaultActionMap;
        GetInputActionMap(nextActionMap)?.Enable();
        currentActionMapName = nextActionMap;
        ExecuteSwitchActionMap(nextActionMap);
        
        GameEvents.OnActionMapChange.Invoke();
    }

    public void ChangeCursorTexture(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.Default:
                if (defaultCursor != null)
                    Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                
                break;
            case CursorType.Sell:
                if (sellCursor != null)
                    Cursor.SetCursor(sellCursor, Vector2.zero, CursorMode.Auto);
                
                break;
            case CursorType.Basic:
                if (basicAttackCursor != null)
                    Cursor.SetCursor(basicAttackCursor, Vector2.zero, CursorMode.Auto);
                
                break;
            case CursorType.Ability:
                if (abilityActiveCursor != null)
                    Cursor.SetCursor(abilityActiveCursor, Vector2.zero, CursorMode.Auto);
                
                break;
        }
    }
    
    public bool IsInputMapActive(string actionMapName) => currentActionMapName == actionMapName;
    public bool IsOverlayActive(string actionMapName) => _overlayStack.Contains(actionMapName);
}
