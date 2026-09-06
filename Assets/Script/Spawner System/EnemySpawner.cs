using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class EnemyData
{
    public string characterName;
    public float change;
    public CharacterSO  characterData;
}

[System.Serializable]
public class EnemySpawnPool
{
    public string day;
    public int dayCount;
    public int raidPoints;
    public float spawnRateMin;
    public float spawnRateMax;

    [Header("Spawn Pool Data")] 
    public List<EnemyData> EnemyDatas = new();
}

[System.Serializable]
public class EnemyRunTimeData
{
    public string characterName;
    public CharacterSO characterData;
    public float enemyHealth;
    public float enemyAttack;
    public int enemyReward;
}


public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnPool> spawnPools = new();
    [SerializeField] private List<CharacterSO> listOfEnemyData = new();
    [SerializeField] private List<Character> activeEnemies = new();
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private Transform endPosition;
    
    [Header("Enemy Data Configuration By Story")]
    [SerializeField] private Transform spawnPointTransform;
    [SerializeField] private List<SpawnPoint> spawnPoints = new();
    
    [Header("Enemy Data Configuration By Story")]
    [SerializeField] private float enemyHealthIncreaseStory;
    [SerializeField] private float enemyAttackIncreaseStory;
    [SerializeField] private float enemyRewardIncreaseStory;
    
    [Header("Enemy Data Configuration By Endless Mode")]
    [SerializeField] private float enemyHealthIncreaseEndless;
    [SerializeField] private float enemyAttackIncreaseEndless;
    [SerializeField] private float enemyRewardIncreaseEndless;
    
    private EnemySpawnPool _selectedSpawnPool;
    private int _raidPoint; 
    private bool _isActive;
    private float _currentSpawnRate;
    private int _spawnCount;
    private HashSet<EnemyRunTimeData> _enemyRunTimeDatas = new();

    // EnemySpawner.cs
    public IReadOnlyList<Character> ActiveEnemies => activeEnemies;
    
    #region Event Configuration

    private void OnEnable()
    {
        GameEvents.OnCharacterDeath.AddListener(HandelEnemyDeath);
        GameEvents.OnChangeToDayLight.AddListener(UpdateEnemyRunTimeData);
        
        GameEvents.OnChangeGameMode.AddListener(EndlessSpawnSet);
    }

    private void OnDisable()
    {
        GameEvents.OnCharacterDeath.RemoveListener(HandelEnemyDeath);
        GameEvents.OnChangeToDayLight.RemoveListener(UpdateEnemyRunTimeData);
        
        GameEvents.OnChangeGameMode.AddListener(EndlessSpawnSet);
    }

    #endregion

    private void Awake()
    {
        spawnPoints.Clear();

        spawnPoints = spawnPointTransform.GetComponentsInChildren<SpawnPoint>(true).ToList();
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
    
    private void Start()
    {
        SetEnemyRunTimeData();
    }

    private void SetEnemyRunTimeData()
    {
        _enemyRunTimeDatas.Clear();

        foreach (var enemyData in listOfEnemyData)
        {
            EnemyRunTimeData newRunTimeData = new EnemyRunTimeData
            {
                characterName = enemyData.characterName,
                characterData = enemyData,
                enemyHealth = enemyData.baseMaxHealth,
                enemyAttack = enemyData.baseAttack,
                enemyReward = enemyData.baseDeathReward
            };
            
            _enemyRunTimeDatas.Add(newRunTimeData);
        }
        
        Debug.Log($"[{name} - (SetEnemyRunTimeData)] total Enemy RunTime Data {_enemyRunTimeDatas.Count}");
    }

    private void UpdateEnemyRunTimeData()
    {
        int dayCount = DayCycleManager.Instance.DayCount;

        foreach (var runTimeData in _enemyRunTimeDatas)
        {
            float baseHealth = runTimeData.characterData.baseMaxHealth;
            float baseAttack = runTimeData.characterData.baseAttack;
            int baseReward = runTimeData.characterData.baseDeathReward;
            
            if (GameManager.Instance.GameMode == GameMode.Story)
            {
                runTimeData.enemyHealth = baseHealth * (1f + (dayCount * (enemyHealthIncreaseStory / 100f)));
                runTimeData.enemyAttack = baseAttack * (1f + (dayCount * (enemyAttackIncreaseStory / 100f)));
                runTimeData.enemyReward = Mathf.RoundToInt(
                    baseReward * (1f + (dayCount * (enemyRewardIncreaseStory / 100f))));
            }
            else
            {
                runTimeData.enemyHealth = baseHealth * (1f + (dayCount * (enemyHealthIncreaseEndless / 100f)));
                runTimeData.enemyAttack = baseAttack * (1f + (dayCount * (enemyAttackIncreaseEndless / 100f))); // Assumes this variable exists
                runTimeData.enemyReward = Mathf.RoundToInt(
                    baseReward * (1f + (dayCount * (enemyRewardIncreaseEndless / 100f)))); // Assumes this variable exists
            }
            
            Debug.LogWarning($"Update RunTime Data {runTimeData.characterName} Health {runTimeData.enemyHealth} Attack{runTimeData.enemyAttack} Reward {runTimeData.enemyReward}");
        }
    }

    private void EndlessSpawnSet(GameMode gameMode)
    {
        if (gameMode == GameMode.Story)
            return;
        
        var pool = spawnPools.Find(x => x.dayCount == 12);
        if (pool == null)
        {
            return;
        }
            
        _selectedSpawnPool = pool;
        _raidPoint = pool.raidPoints;
    }
    
    public void StartSpawning(int dayCount)
    {
        Debug.Log($"[{name} StartSpawning] Start Spawning Enemies");
        if (GameManager.Instance.GameMode == GameMode.Story)
        {
            var pool = spawnPools.Find(x => x.dayCount == dayCount);
            if (pool == null)
            {
                Debug.LogError($"Spawn Pool Not Found for {dayCount}");
                return;
            }
            
            _selectedSpawnPool = pool;
            _raidPoint = pool.raidPoints;
        }
        else
        {
            _raidPoint = _selectedSpawnPool.raidPoints;
        }
        
        _isActive = true;
    }

    public void StopSpawning()
    {
        _isActive = false;
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

    private EnemyRunTimeData GetEnemyRunTimeData(string characterName)
    {
        EnemyRunTimeData data = _enemyRunTimeDatas.FirstOrDefault(enemyData => enemyData.characterName == characterName);
        return data;
    }

    private CharacterSO GetRandomCharacterSO()
    {
        var pool = _selectedSpawnPool.EnemyDatas;
        
        float totalWeight = 0;
        foreach (var enemyData in pool)
        {
            totalWeight  += enemyData.change;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogError($"[{name} (GetRandomCharacterSO)] Spawn pool has no valid weights.");
            return null;
        }
        
        float roll = Random.Range(0, totalWeight);
        float cumulative = 0;

        foreach (var enemyData in pool)
        {
            cumulative += enemyData.change;
            if (roll <= cumulative)
                return enemyData.characterData;
        }
        
        return null;
    }
    
    private void SpawnEnemy()
    {
        if (!_isActive) return;
        
        var enemyData = GetRandomCharacterSO();

        if (!SufficientRaidPoints(enemyData.spawnCost))
            return;
        
        _raidPoint -= enemyData.spawnCost;
        
        int random =  Random.Range(0, spawnPoints.Count);
        var spawnPoint = spawnPoints[random];
        Transform spawnPointTrans =  spawnPoint.transform;
        
        Vector3 offsetSpawnPoint = new Vector3(spawnPointTrans.position.x, 0.7f, spawnPointTrans.position.z);
        
        //spawn the enemy
        if (enemyData.prefabCharacter == null)
        { 
            Debug.LogError($"[{name} (SpawnEnemy)] This CharacterSO Doesn't Have Prefab Character");
            return;
        }
        var enemy = Instantiate(enemyData.prefabCharacter, offsetSpawnPoint, Quaternion.identity, enemyContainer);
        var enemyRunTmeData = GetEnemyRunTimeData(enemy.CharacterData.characterName);
        
        string charID = $"{enemyData.characterName}_{_spawnCount}";
        enemy.InitializeCharacter(charID, endPosition.position, enemyRunTmeData, spawnPoint.RotateSprite);
        
        activeEnemies.Add(enemy);
        
        // Set current 
        _spawnCount++;
        _currentSpawnRate = Random.Range(_selectedSpawnPool.spawnRateMin, _selectedSpawnPool.spawnRateMax);
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
        
        bool noEnemiesLeft      = activeEnemies.Count <= 0;
        bool cannotSpawnMore    = _raidPoint < CheapestEnemyCost();
        
        if (noEnemiesLeft && cannotSpawnMore)
        {
            _isActive = false;
            BattleManager.Instance.WinBattle();
        }
    }
}
