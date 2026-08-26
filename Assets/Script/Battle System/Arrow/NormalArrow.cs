using UnityEngine;

public class NormalArrow : Arrow
{
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask hittableLayerMask;
    
    private SkillCardData _skillCardData;
    private bool _hasReachTarget;
    
    public override void OnSpawnArrow(SkillCardData skillCardData)
    {
        Debug.Log($"{name} Spawn Arrow with radius Explosion = {skillCardData.skillCardSo.explosionData.explosionRadius}");
        _skillCardData = skillCardData;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (_hasReachTarget) return;
        
        int hitLayer = collision.gameObject.layer;
        bool isHittable = (hittableLayerMask.value & (1 << hitLayer)) != 0;
        if (!isHittable) return;
        
        _hasReachTarget = true;
        bool isEnemy = (enemyLayerMask.value & (1 << hitLayer)) != 0;
        
        if (isEnemy)
        {
            IDamageable damageableTarget = collision.gameObject.GetComponent<IDamageable>();
            if (damageableTarget != null)
            {
                HitTarget(damageableTarget);
                return;
            }
        }
        
        Debug.Log($"{name} hit ground, no damage dealt.");
        // TODO: impact VFX/SFX here too, even without damage
        Destroy(gameObject);
    }
    
    private void HitTarget(IDamageable damageableTarget)
    {
        (float damage, bool isCritical) = DamageController.Instance.OnCalculateDamageToEnemy(_skillCardData);
        damageableTarget.TakeDamage(damage, isCritical);
        
        // TODO: hit VFX/SFX here

        Destroy(gameObject);
    }
}
