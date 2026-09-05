using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set ; }

    private InputManager _inputManager;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PauseGame(bool openPausePanel)
    {
        if (openPausePanel)
            GameEvents.OnRequestOpenPanel.Invoke(PanelType.Pause);
        
        if (_inputManager == null)
            _inputManager = InputManager.Instance;
        
        if (DayCycleManager.Instance.DayCycleType == DayCycleType.Day)
            InputManager.Instance.PopInputActionMap();
        else
            _inputManager.ChangeCursorTexture(CursorType.Default);
        
        GameEvents.OnPauseGame.Invoke();
    }

    public void ResumeGame()
    {
        GameEvents.OnRequestClosePanel.Invoke(PanelType.Pause);
        
        if (_inputManager ==  null)
            _inputManager = InputManager.Instance;
        
        if (DayCycleManager.Instance.DayCycleType == DayCycleType.Night)
            _inputManager.ChangeCursorTexture(CursorType.Basic);
        else
            _inputManager.ChangeCursorTexture(CursorType.Default);
        
        GameEvents.OnResumeGame.Invoke();
    }

    public void QuitMainGame()
    { 
        GameManager.Instance.RestartGameManager();
        TransitionManager.Instance.TransitionScene("GamePlay_MainMenu", "FadeOut");
    }
}
