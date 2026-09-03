using System;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
   public static MainMenuManager Instance { get; private set; }

   private void Awake()
   {
      if (Instance != null)
      {
         Destroy(gameObject);
         return;
      }

      Instance = this;
   }

   public void PlayGame()
   {
      TransitionManager.Instance.TransitionScene("GamePlay_MainGame_Test", "FadeOut");
      GameManager.Instance.StartGame();
   }

   public void CreditGame()
   {
      TransitionManager.Instance.TransitionScene("SampleScene", "FadeOut");
         
   }

   public void QuitGame()
   {
      
   }
}
