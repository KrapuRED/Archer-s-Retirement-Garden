using UnityEngine;
using UnityEngine.Events;

public class DialogueEnvironment : MonoBehaviour
{
    [SerializeField] private UnityEvent OnNewEnvironment;
    [SerializeField] private UnityEvent OnPrevEnvironment;
    public string EnvironmentID { get; private set; }

    public void Init() => EnvironmentID = gameObject.name;

    public void ShowEnvironment()
    {
        Debug.LogWarning($"Show Environment {EnvironmentID} {OnNewEnvironment.GetPersistentEventCount()}");
        OnNewEnvironment?.Invoke();
        gameObject.SetActive(true);
    }
    public void HideEnvironment() 
    {
        Debug.LogWarning($"Hide Environment {EnvironmentID}"); 
        OnPrevEnvironment?.Invoke();
        gameObject.SetActive(false);
    }
}
