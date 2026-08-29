using UnityEngine;

public class DamageController : MonoBehaviour
{
    public static DamageController Instance { get; private set; }
    
    private SkillCardManager _skillCardManager;
    private StatusManager  _playerStatusManager;

    private bool _lastHitWasCrit;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
    
    private void Start()
    {
        _skillCardManager = SkillCardManager.Instance;
        _playerStatusManager = StatusManager.Instance;
    }

    private float OnCalculateDamage(float baseAttack, float attackMulti)
    {
        float damage = baseAttack;
        if (attackMulti > 1)
        {
            damage = baseAttack * (attackMulti / 100f);
        }
        
        return damage;
    }

    private (float,bool) OnCalculateCritDamage(float baseDamage, float critRateBoost, float critDamageBoost)
    {
        float critDamage = 0;
        
        float roll = Random.Range(0f, 100f);
        bool isCrit = critRateBoost >= roll;
        
        if (!isCrit) return (critDamage, isCrit);
        
        critDamage = baseDamage * (critDamageBoost / 100f);
        return (critDamage, isCrit);
    }
    
    public void OnCalculateDamageToPlayer(CharacterSO enemyStatusData)
    {
        float damage = OnCalculateDamage(enemyStatusData.baseAttack, 0f);
        (float critDamage, bool isCritical) = OnCalculateCritDamage(damage, enemyStatusData.baseCritRate, enemyStatusData.bassCritDamage);
        
        Debug.Log($"Damage: {damage} + {critDamage} = {critDamage +  damage} isCritical = {isCritical}");
        
        HealthManager.Instance.OnTakeDamage(critDamage + damage, isCritical);
    }

    public (float,bool) OnCalculateDamageToEnemy(SkillCardDataRunTime skillCardDataRunTime)
    {
        if (_playerStatusManager == null || _skillCardManager == null)
        {
            _playerStatusManager = StatusManager.Instance;
            _skillCardManager = SkillCardManager.Instance;
        }
        
        if (skillCardDataRunTime == null)
        {
            Debug.LogError("SkillCardManager is null");
            return (0f , false);
        }
        
        float damage = OnCalculateDamage(_playerStatusManager.AttackBoost,
            skillCardDataRunTime.currentAttackBoost);
        
        (float critDamage, bool isCritical) = OnCalculateCritDamage(damage, _playerStatusManager.CriticalBoostRate, _playerStatusManager.CriticalBoostDamage);
        
        Debug.Log($"OnCalculateDamagePlayer {damage} + {critDamage} = {critDamage +  damage}");
        
        return (damage + critDamage, isCritical);
    }
}