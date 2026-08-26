using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataStateMachine
{
    public string nameStateCondition;
    public StateSO state;
    public ConditionSO condition;
}

public class StateMachine : MonoBehaviour
{
    [SerializeField] private Character ownerChaacter;
    [SerializeField] private List<DataStateMachine> dataStateMachines = new();
    [SerializeField] private StateSO activeState;
    
    private void Update()
    {
        foreach (var data in dataStateMachines)
        {
            if (data.condition.CheckCondition())
            {
                StateSO nextState = data.state;

                if (nextState != activeState)
                {
                    activeState?.ExitState();
                    activeState = nextState;
                    activeState.EnterState();
                }

                break;
            }
        }

        if (activeState != null)
            activeState.ExecuteState(ownerChaacter);
    }

    public void ResetCondition()
    {
        activeState?.ExitState();
        activeState = null;
    }
}
