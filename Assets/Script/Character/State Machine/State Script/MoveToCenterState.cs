using UnityEngine;

[CreateAssetMenu(fileName = "MoveToCenterState", menuName = "State Machine Data/State/MoveToCenterState")]
public class MoveToCenterState : StateSO
{
    public override void EnterState()
    {
        
    }

    public override void ExecuteState(Character character)
    {
        if (character is not EnemyCharacter enemyCharacter)
        {
            Debug.LogWarning($"[{name} - (ExecuteState)] The Character is not EnemyCharacter");
            return;
        }
        
        enemyCharacter.MovementCharacter.MoveCharacterToTarget(enemyCharacter.TargetPosition);
    }

    public override void ExitState()
    {
        
    }

   
}
