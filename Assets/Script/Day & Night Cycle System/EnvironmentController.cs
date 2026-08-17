using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public void ApplyEnvironment(DayCycleType dayCycleType)
    {
        Debug.Log($"[{name} (ApplyEnvironment)] Applying environment for {dayCycleType}");
    }
}
