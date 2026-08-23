using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BarrageExplosionSkill : Skill
{
    [SerializeField] private Transform containerPattern;
    [SerializeField] private List<Transform> barragePattern;
    [SerializeField] private float delayEachTarget;
    [SerializeField] private float delayBeforeNextGroup;
    [SerializeField] private LayerMask enemyLayerMask;
    
    private const int HORIZONTAL_COUNT = 3;
    
    public override void UseSkill(SkillCardData  skillCardData)
    {
        barragePattern.Clear();

        foreach (Transform child in containerPattern)
        {
            barragePattern.Add(child);
            child.gameObject.SetActive(false);
        }

        Debug.Log($"{name} Use Skill! Attack Boost : {skillCardData.currentAttackBoost}");
        StartCoroutine(BarrageSequence(skillCardData));
    }

    private void Explosion(Transform marker, float explosionRadius, SkillCardData skillCardData)
    {
        arrowPrefab.SpawnArrow(explosionRadius);
        marker.gameObject.SetActive(true);

        Collider[] hits = Physics.OverlapSphere(marker.position, explosionRadius, enemyLayerMask);
        foreach (var hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;

            float damage = DamageController.Instance.OnCalculateDamageToEnemy(skillCardData);
            target.TakeDamage(damage);
        }
    }

    private IEnumerator BarrageSequence(SkillCardData skillCardData)
    {
        List<Transform> prevGroup = new List<Transform>();
        
        // --- Vertical group (fires together, after horizontal is done) ---
        List<Transform> vertical = barragePattern.Skip(HORIZONTAL_COUNT).ToList();
        foreach (var marker in vertical)
        {
            Explosion(marker, skillCardData.skillCardSo.explosionData.explosionRadius, skillCardData);
            prevGroup.Add(marker);
            
            yield return new WaitForSeconds(delayEachTarget);
        }
        
        // Wait for the PrevGroup explosions to actually finish before moving on
        yield return new WaitForSeconds(delayBeforeNextGroup);
        foreach (var marker in prevGroup)
        {
            marker.gameObject.SetActive(false);
        }
        
        // --- Horizontal group (fires together) ---
        List<Transform> horizontal = barragePattern.Take(HORIZONTAL_COUNT).ToList();
        foreach (var marker in horizontal)
        {
            Explosion(marker, skillCardData.skillCardSo.explosionData.explosionRadius, skillCardData);
            yield return new WaitForSeconds(delayEachTarget);
        }
        
        yield return new WaitForSeconds(delayBeforeNextGroup);
        Destroy(gameObject);
    }
}
