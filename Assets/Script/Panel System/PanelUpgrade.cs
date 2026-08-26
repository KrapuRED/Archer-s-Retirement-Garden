using UnityEngine;

public class PanelUpgrade : PanelBase
{
    public override void OpenPanel()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        
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
