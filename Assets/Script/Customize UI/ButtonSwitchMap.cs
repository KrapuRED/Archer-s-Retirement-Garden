using UnityEngine;

public class ButtonSwitchMap : MonoBehaviour
{
    [SerializeField] private string actionMapName;

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
            _isSwitched = true;
        }
    }
}
