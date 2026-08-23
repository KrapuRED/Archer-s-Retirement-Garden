using UnityEngine;

public class PanelButton : MonoBehaviour
{
    [SerializeField] private PanelType panelType;

    public void OnClickClosePanel()
    {
        GameEvents.OnRequestClosePanel.Invoke(panelType);
    }

    public void OnClickOpenPanel()
    {
        GameEvents.OnRequestOpenPanel.Invoke(panelType);
    }
}
