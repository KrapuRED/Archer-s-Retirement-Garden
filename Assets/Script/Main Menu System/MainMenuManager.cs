using System;
using UnityEngine;
using MoreMountains.Feedbacks;

public class MainMenuManager : MonoBehaviour
{
   public static MainMenuManager Instance { get; private set; }

   [SerializeField] private MMFeedbacks showFeedback;
   [SerializeField] private Texture2D defaultCursor;
   
   private void Awake()
   {
      if (Instance != null)
      {
         Destroy(gameObject);
         return;
      }

      Instance = this;
   }

   private void Start()
   {
      showFeedback?.PlayFeedbacks();
      Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
   }

   public void PlayGame()
   {
      TransitionManager.Instance.TransitionScene("GamePlay_MainGame", "FadeOut");
      GameManager.Instance.StartGame();
   }

   public void CreditGame()
   {
      TransitionManager.Instance.TransitionScene("Credit", "FadeOut");
         
   }

   public void QuitGame()
   {
      Application.Quit();
   }
}
