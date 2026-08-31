using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DataDialogueEnvironment
{
    public string environmentID;
    public DialogueEnvironment environmentData;
}

public class DialogueEnvironmentController : MonoBehaviour
{
    [SerializeField] private GameObject mainGameUI;
    
    [Header("Dialogue Environment References")]
    [SerializeField] private Transform dialogueEnvironmentContainer;
    [SerializeField] private List<DataDialogueEnvironment> environments = new List<DataDialogueEnvironment>();
    
    private void Awake()
    {
        environments.Clear();

        foreach (var environment in dialogueEnvironmentContainer.GetComponentsInChildren<DialogueEnvironment>(true).ToList())
        {
            environment.Init();
            
            DataDialogueEnvironment dataDialogueEnvironment = new DataDialogueEnvironment
            {
                environmentID = environment.EnvironmentID,
                environmentData = environment
            };
            
            environments.Add(dataDialogueEnvironment);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnChangeEnvironment.AddListener(DialogueEnvironmentHandler);
    }

    private void OnDisable()
    {
        GameEvents.OnChangeEnvironment.RemoveListener(DialogueEnvironmentHandler);
    }

    private void DialogueEnvironmentHandler(string environmentID)
    {
        if (string.IsNullOrEmpty(environmentID))
        {
            HideEnvironment();
            return; 
        }
        
        foreach (var environment in environments)
        {
            if (environment.environmentID == environmentID)
            {
                mainGameUI.SetActive(false);
                environment.environmentData.ShowEnvironment();
            }
            else
            {
                environment.environmentData.HideEnvironment();
            }
        }
    }

    private void HideEnvironment()
    {
        mainGameUI.SetActive(true);
        foreach (var environment in environments)
        {
            if (environment != null)
            {
                environment.environmentData.HideEnvironment();
            }
        }
    }
}
