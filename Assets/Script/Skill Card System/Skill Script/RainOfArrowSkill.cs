using System.Collections;
using UnityEngine;

public class RainOfArrowSkill : Skill
{
    [SerializeField] private Transform previewTarget;
    [SerializeField] private float damageRadius = 3f;
    [SerializeField] private float tickInterval = 0.5f;
    [SerializeField] private LayerMask damageLayerMask;
    
    private bool _isAbleDealDamage;
    
    public override void UseSkill(SkillCardData  skillCardData)
    {
        arrowPrefab.OnSpawnArrow(skillCardData);
        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardData.currentAttackBoost}");
        
        _isAbleDealDamage = true;
        StartCoroutine(RainOfArrow(skillCardData));
    }

    private IEnumerator RainOfArrow(SkillCardData  skillCardData)
    {
        float elapsed = 0f;
        float duration = skillCardData.currentDuration;
        
        // 1) Apply damage in duration
        while (_isAbleDealDamage && elapsed < duration)
        {
            DealDamageInArea(skillCardData);
            
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }
        
        yield return new WaitForSeconds(duration);
        _isAbleDealDamage = false;
        // 2) Exit
        Destroy(gameObject);
    }

    private void DealDamageInArea(SkillCardData  skillCardData)
    {
        Collider[] colliders = Physics.OverlapSphere(previewTarget.position, damageRadius, damageLayerMask);

        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                (float damage, bool isCritical) = DamageController.Instance.OnCalculateDamageToEnemy(skillCardData);
                damageable.TakeDamage(damage, isCritical);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (previewTarget == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(previewTarget.position, damageRadius);
    }
}
