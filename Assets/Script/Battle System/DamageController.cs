using UnityEngine;

public class DamageController : MonoBehaviour
{
    public static DamageController Instance { get; private set; }
    
    private SkillCardManager _skillCardManager;
    private StatusManager  _playerStatusManager;

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

    private float OnCalculateCritDamage()
    {
        return 0;
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
        
        float critDamage = OnCalculateCritDamage();
        
        return damage + critDamage;
    }
}