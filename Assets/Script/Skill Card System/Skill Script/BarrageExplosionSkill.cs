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

    private void ArrowSpawn(Transform marker, SkillCardData skillCardData)
    {
        Vector3 spawnPosition = new Vector3(marker.position.x, marker.position.y + offsetSpawnArrow, marker.position.z);
        var arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity, containerPattern);
        if (arrow == null)
        {
            Destroy(arrow);
            return;
        }
        
        arrow.OnSpawnArrow(skillCardData);
        marker.gameObject.SetActive(true);
    }

    private IEnumerator BarrageSequence(SkillCardData skillCardData)
    {
        List<Transform> prevGroup = new List<Transform>();
        
        // --- Vertical group (fires together, after horizontal is done) ---
        List<Transform> vertical = barragePattern.Skip(HORIZONTAL_COUNT).ToList();
        foreach (var marker in vertical)
        {
            ArrowSpawn(marker, skillCardData);
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
            ArrowSpawn(marker, skillCardData);
            yield return new WaitForSeconds(delayEachTarget);
        }
        
        yield return new WaitForSeconds(delayBeforeNextGroup);
        Destroy(gameObject);
    }
}
