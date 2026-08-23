using UnityEngine;

public class ExplosionArrow : Arrow
{
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask hittableLayerMask;
    
    private float _radiusExplosion;
    private SkillCardData _skillCardData;
    private bool _hasExploded;
    
    public override void OnSpawnArrow(SkillCardData skillCardData)
    {
        _skillCardData = skillCardData;
        _radiusExplosion = skillCardData.skillCardSo.explosionData.explosionRadius;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasExploded) return;
        
        int hitLayer = collision.gameObject.layer;
        bool isHittable = (hittableLayerMask.value & (1 << hitLayer)) != 0;
        if (!isHittable) return;
        
        Explosion();
    }

    private void Explosion()
    {
        _hasExploded = true;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, _radiusExplosion, enemyLayerMask);
        foreach (var hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;

            float damage = DamageController.Instance.OnCalculateDamageToEnemy(_skillCardData);
            target.TakeDamage(damage);
        }

        // TODO: explosion VFX/SFX here

        Destroy(gameObject);
    }
}
