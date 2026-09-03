using UnityEngine;

public class PanelUpgrade : PanelBase
{
    [SerializeField] private UpgradeRefreshButton refreshButton;
    
    public override void OpenPanel()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        
        refreshButton.DisplayRefreshButton();
        
        InputManager.Instance.ChangeCursorTexture(CursorType.Default);
        UpgradeCardManager.Instance.OnShowRandomUpgradeCard();
    }

    public override void ClosePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        
        DayCycleManager.Instance.AddDayCount();
        DayCycleManager.Instance.UpdateCycleManager();
    }
}
