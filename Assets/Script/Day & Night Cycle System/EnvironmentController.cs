using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [SerializeField] private string dayActionMap;
    [SerializeField] private string nightActionMap;
    
    public void ApplyEnvironment(DayCycleType dayCycleType)
    {
        if (dayCycleType == DayCycleType.Day)
        {
            Debug.Log($"Day {dayCycleType} has been applied");
            InputManager.Instance.PopInputActionMap();
        }
        else if (dayCycleType == DayCycleType.Night)
        {
            InputManager.Instance.SwitchInputMap(nightActionMap);
        }
    }
}
