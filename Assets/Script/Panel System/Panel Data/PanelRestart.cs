using UnityEngine;
using MoreMountains.Feedbacks;


public class PanelRestart : PanelBase
{
   [SerializeField] private MMFeedbacks showPanelFeedback;
   [SerializeField] private MMFeedbacks hidePanelFeedback;
   
   public override void OpenPanel()
   {
      showPanelFeedback?.PlayFeedbacks();
      canvasGroup.interactable = true;
   }

   public override void ClosePanel()
   {
      hidePanelFeedback?.PlayFeedbacks();
      canvasGroup.interactable = false;
      canvasGroup.interactable = false;
   }
   
   public void OnRestart()
   {
      GameManager.Instance.OnRestartGame();
   }

   public void OnQuit()
   {
      
   }
}
