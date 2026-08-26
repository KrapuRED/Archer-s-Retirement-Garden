using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class EnemySpawnPool
{
    public string day;
    public int dayCount;
    public int raidPoints;
    public float spawnRate;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnPool> spawnPools = new();
    [SerializeField] private List<CharacterSO> listOfEnemyData = new();
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private List<Character> activeEnemies = new();
    [SerializeField] private Transform enemyContainer;
    
    [SerializeField] private Transform endPosition;
    
    private EnemySpawnPool _selectedSpawnPool;
    private int _raidPoint; 
    private bool _isActive;
    private float _currentSpawnRate;
    private int _spawnCount;

    #region Event Configuration

    private void OnEnable()
    {
        GameEvents.OnCharacterDeath.AddListener(HandelEnemyDeath);
    }

    private void OnDisable()
    {
        GameEvents.OnCharacterDeath.RemoveListener(HandelEnemyDeath);
    }

    #endregion
    
    public void StartSpawning(int dayCount)
    {
        Debug.Log($"[{name} StartSpawning] Start Spawning Enemies");
        
        var pool = spawnPools.Find(x => x.dayCount == dayCount);
        if (pool == null)
        {
            Debug.LogError($"Spawn Pool Not Found for {dayCount}");
            return;
        }
        
        _isActive = true;
        _selectedSpawnPool = pool;
        _raidPoint = pool.raidPoints;
    }

    public void StopSpawning()
    {
        _isActive = false;
        _selectedSpawnPool = null;
        _currentSpawnRate = 0;
        
         
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }
        activeEnemies.Clear();
    }
    
    private bool SufficientRaidPoints(int spawnCost)
    {
        return spawnCost <= _raidPoint;
    }

    private void Update()
    {
        if (!_isActive) return;
        
        if (_currentSpawnRate <= 0)
        {
            SpawnEnemy();
            return;
        }
        
        _currentSpawnRate -= Time.deltaTime;
    }

    private void SpawnEnemy()
    {
        if (!_isActive) return;

        int randomIndex = Random.Range(0, listOfEnemyData.Count);
        var enemyData = listOfEnemyData[randomIndex];

        if (!SufficientRaidPoints(enemyData.spawnCost))
            return;
        
        _raidPoint -= enemyData.spawnCost;
        
        int random =  Random.Range(0, spawnPoints.Count);
        var spawnPoint = spawnPoints[random];
        Vector3 offsetSpawnPoint = new Vector3(spawnPoint.position.x, 0.7f, spawnPoint.position.z);
        
        //spawn the enemy
        if (enemyData.prefabCharacter == null)
        { 
            Debug.LogError($"[{name} (SpawnEnemy)] This CharacterSO Doesn't Have Prefab Character");
            return;
        }
        var enemy = Instantiate(enemyData.prefabCharacter, offsetSpawnPoint, Quaternion.identity, enemyContainer);
        string charID = $"{enemyData.characterName}_{_spawnCount}";
        enemy.InitializeCharacter(charID, endPosition.position);
        
        activeEnemies.Add(enemy);
        
        // Set current 
        _spawnCount++;
        _currentSpawnRate = _selectedSpawnPool.spawnRate;
    }

    private int CheapestEnemyCost()
    {
        int min = int.MaxValue;
        foreach (var data in listOfEnemyData)
            if (data.spawnCost < min) min = data.spawnCost;
        return min;
    }

    private void HandelEnemyDeath(Character enemy)
    {
        var enemyData = activeEnemies.Find(x => x.CharacterID == enemy.CharacterID);
        if (enemyData == null)
        {
            Debug.LogWarning($"[{name} (HandelEnemyDeath)] There are not enemies in the active enemies list with {enemy.CharacterID}");
            return;
        }

        activeEnemies.Remove(enemyData);
        CheckForWin();
    }
    
    private void CheckForWin()
    {
        if (!_isActive) return;
        
        bool noEnemiesLeft  = activeEnemies.Count <= 0;
        bool cannotSpawnMore    = _raidPoint < CheapestEnemyCost();
        
        if (noEnemiesLeft && cannotSpawnMore)
        {
            _isActive = false;
            BattleManager.Instance.WinBattle();
        }
    }
}
