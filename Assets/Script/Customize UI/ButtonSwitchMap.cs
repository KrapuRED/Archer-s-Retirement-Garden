using UnityEngine;

public class ButtonSwitchMap : MonoBehaviour
{
    [SerializeField] private string actionMapName;
    [SerializeField] private CursorType cursorType;
    
    private bool _isSwitched;
    
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
