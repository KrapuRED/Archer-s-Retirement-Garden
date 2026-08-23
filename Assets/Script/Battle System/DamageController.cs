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

    private float OnCalculateDamage(float baseAttack, float attackMulti = 1)
    {
        float damage = baseAttack * (attackMulti / 100f);
        return damage;
    }

    private float OnCalculateCritDamage(float baseDamage)
    {
        float critDamage = 0;
        
        float roll = Random.Range(0f, 100f);
        bool isCrit = _playerStatusManager.CriticalBoostRate >= roll;
        
        if (!isCrit) return critDamage;
        
        critDamage = baseDamage * (_playerStatusManager.CriticalBoostDamage / 100f);
        return critDamage;
    }
    
    public void OnCalculateDamagePlayer(CharacterSO enemyStatusData)
    {

    }

    public float OnCalculateDamageToEnemy(SkillCardData skillCardData)
    {
        if (_playerStatusManager == null || _skillCardManager == null)
        {
            _playerStatusManager = StatusManager.Instance;
            _skillCardManager = SkillCardManager.Instance;
        }
        
        if (skillCardData == null)
        {
            Debug.LogError("SkillCardManager is null");
            return 0f;
        }
        
        float damage = OnCalculateDamage(_playerStatusManager.AttackBoost,
            skillCardData.currentAttackBoost);
        
        float critDamage = OnCalculateCritDamage(damage);
        
        Debug.Log($"OnCalculateDamagePlayer {damage} + {critDamage} = {critDamage +  damage}");
        
        return damage + critDamage;
    }
}