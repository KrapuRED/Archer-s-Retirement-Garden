using System;
using UnityEngine;

public class ButtonSwitchMap : MonoBehaviour
{
    [SerializeField] private string actionMapName;
    [SerializeField] private CursorType cursorType;
    
    private bool _isSwitched;

    private void OnEnable()
    {
        GameEvents.OnActionMapChange.AddListener(SwicthBackButton);
    }

    private void OnDisable() => OnRemoverListener();
    private void OnDestroy() => OnRemoverListener();

    private void OnRemoverListener()
    {
        GameEvents.OnActionMapChange.RemoveListener(SwicthBackButton);
    }
    
    private void SwicthBackButton() => _isSwitched = false;
    
    public void SwitchMap()
    {
        if (_isSwitched)
        {
            InputManager.Instance.PopInputActionMap();
            _isSwitched = false;
        }
        else
        {
            InputManager.Instance.SwitchInputMap(actionMapName);
            InputManager.Instance.ChangeCursorTexture(cursorType);
            _isSwitched = true;
        }
    }
}
