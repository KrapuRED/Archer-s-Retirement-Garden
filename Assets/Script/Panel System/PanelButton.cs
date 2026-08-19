using UnityEngine;

public class PanelButton : MonoBehaviour
{
    [SerializeField] private PanelType panelType;

    public void OnClick()
    {
        GameEvents.OnRequestClosePanel.Invoke(panelType);
    }
}
