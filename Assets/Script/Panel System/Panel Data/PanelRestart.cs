using UnityEngine;

public class PanelRestart : PanelBase
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
   
   public void OnRestart()
   {
      GameManager.Instance.OnRestartGame();
   }
}
