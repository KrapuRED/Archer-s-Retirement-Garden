using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    
    [SerializeField] private EnemySpawner enemySpawner;
    public EnemySpawner EnemySpawner => enemySpawner;
    
    public bool IsBattleActive { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void HandelBattle(DayCycleType dayType)
    {
        if (dayType == DayCycleType.Night)
        {
            Debug.Log($"[{nameof(BattleManager)}] Handel Battle started]");
            StartBattle();
        }
        else
        {
            Debug.Log($"[{nameof(BattleManager)}] Handel Battle ended]");
            EndBattle();
        }
    }
    
    private void StartBattle()
    {
        if (IsBattleActive) return;
        
        IsBattleActive = true;

        int dayCount = DayCycleManager.Instance.DayCount;
        enemySpawner.StartSpawning(dayCount);
    }

    private void EndBattle()
    {
        if (!IsBattleActive) return;
        
        InputManager.Instance.PopInputActionMap();
        enemySpawner.StopSpawning();
        IsBattleActive = false;
    }

    public void WinBattle()
    {
        Debug.Log($"[{nameof(BattleManager)}] Battle Won");
        EndBattle();
        
        //Show Upgrade Panel
        GameEvents.OnRequestOpenPanel.Invoke(PanelType.Upgrade);
    }

    public void LoseBattle()
    {
        Debug.Log($"[{nameof(BattleManager)}] Battle Lost");
        EndBattle();
        
        //Back tp day in the same day
        GameManager.Instance.OnGameOver();
    }
}
