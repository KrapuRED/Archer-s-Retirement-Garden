using UnityEngine;

public class PanelEndGame : PanelBase
{
    public override void OpenPanel()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public override void ClosePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void ContinueEndless()
    {
        GameEvents.OnRequestClosePanel.Invoke(panelType);
        GameManager.Instance.ChangeGameMode(GameMode.Endless);
    }

    public void QuitAndCredit()
    {
        GameEvents.OnRequestClosePanel.Invoke(panelType);
        GameManager.Instance.EndGame();
    }
}
