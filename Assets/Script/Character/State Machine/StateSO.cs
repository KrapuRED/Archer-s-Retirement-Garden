using UnityEngine;

[CreateAssetMenu(fileName = "StateSO", menuName = "Scriptable Objects/StateSO")]
public abstract class StateSO : ScriptableObject
{
    public abstract void EnterState();

    public abstract void ExecuteState(Character character);

    public abstract void ExitState();
}
