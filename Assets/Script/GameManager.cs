using System;
using UnityEngine;

[System.Serializable]
public enum GameMode
{
    Story,
    Endless
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private GameMode gameMode;
    
    [Header("Lose Reward")]
    [SerializeField] private float recoverHealth;
    [SerializeField] private int loseReward;
    
    public GameMode GameMode => gameMode;

    public bool IsGameActive { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        
        IsGameActive = true;
        DontDestroyOnLoad(gameObject);
    }

    public void OnGameOver()
    {
        IsGameActive = false;
        
        DayCycleManager.Instance.UpdateCycleManager();
        GameEvents.OnRequestOpenPanel.Invoke(PanelType.Lose);
    }
    
    public void OnRestartGame()
    {
        CurrencyManager.Instance.AddCurrency(loseReward);
        HealthManager.Instance.OnTakeHeal(recoverHealth);
        
        IsGameActive = true;
    }
}
