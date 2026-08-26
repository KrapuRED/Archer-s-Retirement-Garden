using UnityEngine;

[CreateAssetMenu(fileName = "IsMovingToMiddleCondition", menuName = "State Machine Data/Condition/IsMovingToMiddleCondition")]
public class IsMovingToMiddleCondition : ConditionSO
{
    public override bool CheckCondition(Character character)
    {
        //Reach Ground Move to middle
        if (character is not EnemyCharacter enemyCharacter)
        {
            Debug.LogWarning($"[{name} - (ExecuteState)] The Character is not EnemyCharacter");
            return false;
        }
        return enemyCharacter.IsGrounded;
    }
}
