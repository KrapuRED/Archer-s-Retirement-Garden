using System.Collections;
using UnityEngine;

public class RainOfArrowSkill : Skill
{
    [SerializeField] private Transform previewTarget;
    [SerializeField] private float damageRadius = 3f;
    [SerializeField] private float tickInterval = 0.5f;
    [SerializeField] private LayerMask damageLayerMask;
    
    [SerializeField] private GameObject visualEffectPrefab;
    
    private bool _isAbleDealDamage;
    
    public override void UseSkill(SkillCardDataRunTime  skillCardDataRunTime)
    {
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardDataRunTime.currentAttackBoost}");
        
        _isAbleDealDamage = true;
        StartCoroutine(RainOfArrow(skillCardDataRunTime));
    }

    private IEnumerator RainOfArrow(SkillCardDataRunTime  skillCardDataRunTime)
    {
        float elapsed = 0f;
        float duration = skillCardDataRunTime.currentDuration;
        
        visualEffectPrefab.SetActive(true);
        
        // 1) Apply damage in duration
        while (_isAbleDealDamage && elapsed < duration)
        {
            DealDamageInArea(skillCardDataRunTime);
            
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }
        
        yield return new WaitForSeconds(0.5f);
        
        visualEffectPrefab.SetActive(false);
        _isAbleDealDamage = false;
        
        // 2) Exit
        Destroy(gameObject);
    }

    private void DealDamageInArea(SkillCardDataRunTime  skillCardDataRunTime)
    {
        Collider[] colliders = Physics.OverlapSphere(previewTarget.position, damageRadius, damageLayerMask);

        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                (float damage, bool isCritical) = DamageController.Instance.OnCalculateDamageToEnemy(skillCardDataRunTime);
                damageable.TakeDamage(damage, isCritical);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (previewTarget == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(previewTarget.position, damageRadius);
    }
}
