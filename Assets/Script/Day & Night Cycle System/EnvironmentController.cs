using UnityEngine;
using UnityEngine.Events;

public class EnvironmentController : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private UnityEvent OnDayEnvironment;
    [SerializeField] private UnityEvent OnNightEnvironment;
    
    [Header("Action Map Controls")]
    [SerializeField] private string dayActionMap;
    [SerializeField] private string nightActionMap;
    
    public void ApplyEnvironment(DayCycleType dayCycleType)
    {
        if (dayCycleType == DayCycleType.Day)
        {
            OnDayEnvironment?.Invoke();
            
            InputManager.Instance.PopInputActionMap();
        }
        else if (dayCycleType == DayCycleType.Night)
        {
            OnNightEnvironment?.Invoke();
            
            InputManager.Instance.SwitchInputMap(nightActionMap);
        }
        
        if (BattleManager.Instance != null)
            BattleManager.Instance.HandelBattle(dayCycleType);
    }
}
