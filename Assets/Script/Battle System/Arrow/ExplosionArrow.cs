using UnityEngine;
using System.Collections;

public class ExplosionArrow : Arrow
{
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask hittableLayerMask;
    [SerializeField] private GameObject explosionVFX;
    
    private float _radiusExplosion;
    private SkillCardDataRunTime _skillCardDataRunTime;
    private bool _hasExploded;
    
    public override void OnSpawnArrow(SkillCardDataRunTime skillCardDataRunTime)
    {
        _skillCardDataRunTime = skillCardDataRunTime;
        _radiusExplosion = skillCardDataRunTime.skillCardSo.explosionData.explosionRadius;
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

            (float damage, bool isCritical) = DamageController.Instance.OnCalculateDamageToEnemy(_skillCardDataRunTime);
            target.TakeDamage(damage, isCritical);
        }

        // TODO: explosion VFX/SFX here
        explosionVFX.SetActive(true);

        StartCoroutine(DestroyAfterTime(10));
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        explosionVFX.SetActive(true);
        
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    
}
