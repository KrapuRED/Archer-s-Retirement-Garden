using UnityEngine;
using System.Collections.Generic;

public class AutoAttackSkill : Skill
{
    public float offsetX;
    
    public override void UseSkill(SkillCardData skillCardData)
    {
        List<Character> activeEnemies = new List<Character>();
        
        for (int i = 0; i <= skillCardData.currentMaxTarget; i++)
        {
            var enemy = GetRandomActiveEnemy();
            activeEnemies.Add(enemy);
        }
    }

    private Character GetRandomActiveEnemy()
    {
        var enemies = BattleManager.Instance.EnemySpawner.ActiveEnemies;
        if (enemies == null || enemies.Count == 0) return null;

        int index = Random.Range(0, enemies.Count);
        return enemies[index];
    }
    
    private void AutoAttack(List<Character> activeEnemies)
    {

    }
}
