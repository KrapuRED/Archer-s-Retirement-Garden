using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] private EnemySpawner enemySpawner;
    
    private bool _isBattleActive;
    
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
        if (_isBattleActive) return;
        
        _isBattleActive = true;

        int dayCount = DayCycleManager.Instance.DayCount;
        enemySpawner.StartSpawning(dayCount);
    }

    private void EndBattle()
    {
        if (!_isBattleActive) return;
        
        enemySpawner.StopSpawning();
        _isBattleActive = false;
    }
    
    public void WinBattle()
    {
        Debug.Log($"[{nameof(BattleManager)}] Battle Won");
        EndBattle();
        
        //Continue to next day
    }
    
    public void LoseBattle()
    {
        Debug.Log($"[{nameof(BattleManager)}] Battle Lost");
        EndBattle();
        
        //Back tp day in the same day
    }
}
