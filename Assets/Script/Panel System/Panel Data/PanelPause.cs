using UnityEngine;

public class PanelPause : PanelBase
{
   public override void OpenPanel()
   {
      canvasGroup.alpha = 1;
      canvasGroup.blocksRaycasts = true;
      canvasGroup.interactable = true;
      
      PauseManager.Instance.PauseGame(false);
   }

   public override void ClosePanel()
   {
      canvasGroup.alpha = 0;
      canvasGroup.blocksRaycasts = false;
      canvasGroup.interactable = false;
      
      PauseManager.Instance.ResumeGame();
   }
}
