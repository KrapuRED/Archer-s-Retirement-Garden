using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set ; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PauseGame()
    {
        GameEvents.OnRequestOpenPanel.Invoke(PanelType.Pause);
        GameEvents.OnPauseGame.Invoke();
    }

    public void ResumeGame()
    {
        GameEvents.OnRequestClosePanel.Invoke(PanelType.Pause);
        GameEvents.OnResumeGame.Invoke();
    }
}
