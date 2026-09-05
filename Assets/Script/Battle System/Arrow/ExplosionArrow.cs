using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

public class ExplosionArrow : Arrow
{
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask hittableLayerMask;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private AudioClip explosionSoundEffect;
    
    private float _radiusExplosion;
    private SkillCardDataRunTime _skillCardDataRunTime;
    private bool _hasExploded;
    private CapsuleCollider _collider;
    private SpriteRenderer _spriteRenderer;
    
    public override void OnSpawnArrow(SkillCardDataRunTime skillCardDataRunTime)
    {
        if (_collider == null) _collider = GetComponent<CapsuleCollider>();
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        
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
        
        _spriteRenderer.enabled = false;
        _collider.isTrigger = true;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, _radiusExplosion, enemyLayerMask);
        foreach (var hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;

            (float damage, bool isCritical) = DamageController.Instance.OnCalculateDamageToEnemy(_skillCardDataRunTime);
            target.TakeDamage(damage, isCritical);
        }

        // TODO: explosion VFX/SFX here
        
        StartCoroutine(DestroyAfterTime(5));
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        explosionVFX.SetActive(true);
        MMSoundManagerSoundPlayEvent.Trigger(explosionSoundEffect, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
        
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    
}
