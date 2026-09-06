using UnityEngine;

public class SceneChangeHelper : Helper
{
   [SerializeField] private string sceneName;
   
   public override void UseHelper()
   {
      TransitionManager.Instance.TransitionScene(sceneName, "FadeOut");
   }
}
