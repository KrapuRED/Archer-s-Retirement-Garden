using MoreMountains.Feedbacks;
using UnityEngine;
using MoreMountains.Tools;

public class PanelUpgrade : PanelBase
{
    [SerializeField] private UpgradeRefreshButton refreshButton;
    
    [SerializeField] private MMFeedbacks showPanelFeedback;
    [SerializeField] private MMFeedbacks hidePanelFeedback;
    
    public override void OpenPanel()
    {
        showPanelFeedback?.PlayFeedbacks();
        canvasGroup.interactable = true;
        
        refreshButton.DisplayRefreshButton();
        
        InputManager.Instance.ChangeCursorTexture(CursorType.Default);
        UpgradeCardManager.Instance.OnShowRandomUpgradeCard();
    }

    public override void ClosePanel()
    {
        hidePanelFeedback?.PlayFeedbacks();
        canvasGroup.interactable = false;
        
        DayCycleManager.Instance.AddDayCount();
        DayCycleManager.Instance.UpdateCycleManager();
    }
}
