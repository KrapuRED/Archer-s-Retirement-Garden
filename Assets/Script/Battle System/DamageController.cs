using System;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    private SkillCardManager _skillCardManager;
    private StatusManager  _playerStatusManager;

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
        HealthManager.Instance.OnTakeDamage(enemyStatusData.baseAttack);
    }

    public void OnCalculateDamageEnemy()
    {
        float damage = OnCalculateDamage(_playerStatusManager.AttackBoost,
            _skillCardManager.SelectedSkillCard.AttackBoostSkillCard);
        float critDamage = OnCalculateCritDamage();
        
        float finalDamage = damage + critDamage;
        Debug.Log(finalDamage);
    }
}