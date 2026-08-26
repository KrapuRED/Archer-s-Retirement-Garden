using UnityEngine;

[CreateAssetMenu(fileName = "IsMovingToMiddleCondition", menuName = "State Machine Data/Condition/IsMovingToMiddleCondition")]
public class IsMovingToMiddleCondition : ConditionSO
{
    public override bool CheckCondition()
    {
        return true;
    }
}
