using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataStateMachine
{
    public string nameStateCondition;
    public StateSO state;
    public ConditionSO condition;
}

public class StateMachine : MonoBehaviour, IPauseable
{
    [SerializeField] private Character ownerCharacter;
    [SerializeField] private List<DataStateMachine> dataStateMachines = new();
    [SerializeField] private StateSO activeState;
    
    public bool IsPaused { get; set; }

    private void OnEnable()
    {
        GameEvents.OnPauseGame.AddListener(Pause);
        GameEvents.OnResumeGame.AddListener(Resume);
    }

    private void OnDisable() => OnRemoveListeners();

    private void OnDestroy() => OnRemoveListeners();

    private void OnRemoveListeners()
    {
        GameEvents.OnPauseGame.RemoveListener(Pause);
        GameEvents.OnResumeGame.RemoveListener(Resume);
    }

    private void Update()
    {
        if (IsPaused || ownerCharacter.IsDead)
            return;
        
        foreach (var data in dataStateMachines)
        {
            if (data.condition.CheckCondition(ownerCharacter))
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
            activeState.ExecuteState(ownerCharacter);
    }

    public void ResetCondition()
    {
        activeState?.ExitState();
        activeState = null;
    }
    
    public void Pause()
    {
        IsPaused = true;
    }
    
    public void Resume()
    {
        IsPaused = false;
    }
}
