using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AutoAttackSkill : Skill
{
    [SerializeField] private string skillName;
    [SerializeField] private float offset;
    [SerializeField] private GameObject previewtarget;
    
    private List<GameObject> _previewInstances = new ();
    
    public string SkillName => skillName;
    
    public override void UseSkill(SkillCardDataRunTime skillCardDataRunTime)
    {
        if (skillCardDataRunTime == null)
        { 
            Debug.LogError($"[{name} - (UseSkill)] Skill CardData RunTime is NULL!");
            return;
        }

        var activeLocaction = GetRandomActiveEnemiesTransform(skillCardDataRunTime.currentMaxTarget);
        AutoAttack(activeLocaction, skillCardDataRunTime);
    }

    private List<Transform> GetRandomActiveEnemiesTransform(int maxTarget)
    {
        var listActiveEnemies = BattleManager.Instance.EnemySpawner.ActiveEnemies;
        List<Transform> result = new List<Transform>();

        if (listActiveEnemies == null || listActiveEnemies.Count == 0)
            return result;

        var shuffled = listActiveEnemies
            .Where(e => e != null)
            .OrderBy(_ => Random.value)
            .Take(maxTarget);

        foreach (var enemy in shuffled)
            result.Add(enemy.transform);
        
        return result;
    }
    
    private void ArrowSpawn(Transform marker, SkillCardDataRunTime skillCardDataRunTime)
    {
        Vector3 spawnPosition = new Vector3(marker.position.x, marker.position.y + offsetSpawnArrow, marker.position.z);
        
        var arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity, this.transform);
        if (arrow == null)
        {
            Destroy(arrow);
            return;
        }

        arrow.OnSpawnArrow(skillCardDataRunTime);
        marker.gameObject.SetActive(false);
    }
    
    private void AutoAttack(List<Transform> activeEnemies, SkillCardDataRunTime skillCardDataRunTime)
    {
        // 1) Spawn Targeting
        foreach (var activeEnemy in activeEnemies)
        {
            Vector3 offsetSpawn = new Vector3(activeEnemy.position.x + offset, 0f, activeEnemy.position.z + offset);
            GameObject target = Instantiate(previewtarget, offsetSpawn, Quaternion.identity, this.transform);

            ArrowSpawn(target.transform, skillCardDataRunTime);
        }
    }
}
