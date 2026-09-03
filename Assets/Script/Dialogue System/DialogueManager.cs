using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DialogueDataRunTime
{
    public string dialogueName;
    public int dayDialogue;
    public List<DialogueSO> dialogueData = new();
    public bool isComplete;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private bool dialogueStart;
    [SerializeField] private DialogueCharacterController dialogueCharacterController;
    [SerializeField] private List<DialogueDataRunTime> dialogueDataRunTimes = new();
    
    private DialogueDataRunTime _selectedDialogueDataRunTime;
    private DialogueSO _currentDialogueData;
    private int _dialogueDataIndex;
    private int _dialogueIndex;

    public bool IsDialogueRunning { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnChangeToDayLight.AddListener(StartDialogue);
        GameEvents.OnStartDialogue.AddListener(StartDialogue);
    }

    private void OnDisable()
    {
        GameEvents.OnChangeToDayLight.RemoveListener(StartDialogue);
        GameEvents.OnStartDialogue.RemoveListener(StartDialogue);
        
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ContinueDialogue();
    }

    private void DisplayDialogue()
    {
        Debug.Log($"Dialogue Data Index : {_dialogueDataIndex} Dialogue Index : {_dialogueIndex}" );
        
        var line = _currentDialogueData.dialogueLines[_dialogueIndex];
        
        if (!string.IsNullOrEmpty(line.characterName))
            dialogueCharacterController.ShowCharacter(line.characterName);
        
        GameEvents.OnDisplayDialogue.Invoke(line.characterName, line.dialogueLine);
    }

    private bool IsMultipleDialogue(DialogueDataRunTime dialogueDataRunTime)
    {
        return dialogueDataRunTime.dialogueData.Count > 1;
    }

    private void ChangeDialogueData()
    {
        
        if (dialogueDataRunTimes.Count > 0)
            _dialogueDataIndex++;
        
        dialogueCharacterController.HideAllCharacters();
        _currentDialogueData  = _selectedDialogueDataRunTime.dialogueData[_dialogueDataIndex];
        
        string environment = $"Environment Dialogue - {_currentDialogueData.locationDialogue}";

        if (TransitionManager.Instance == null)
        {
            Debug.LogWarning($"[{name} (StartDialogue)] Transition Manager is MISING or NULL!");
            return;
        }
            
        TransitionManager.Instance.TransitionDialogueEnvironment(environment, "FadeOut");
    }

    private void StartDialogue()
    {
        if (IsDialogueRunning) return;
     
        Debug.LogWarning($"[{name} (StartDialogue)] This dialogueData is running.");
        int dayCount = DayCycleManager.Instance.DayCount;
        var dialogueData = dialogueDataRunTimes.Find(x => x.dayDialogue == dayCount);
        if (dialogueData == null)
        {
            Debug.LogWarning($"[{name} (StartDialogue)] There are no dialogue data for {dayCount} day!");
            return;
        }

        if (dialogueData.isComplete)
        {
            Debug.LogWarning($"[{name} (StartDialogue)] This dialogueData is complete! {dialogueData.dialogueName}");
            return;
        }
        
        _selectedDialogueDataRunTime  = dialogueData;
        IsDialogueRunning = true;
        _dialogueIndex = -1;
        _dialogueDataIndex = -1;
        
        Debug.Log($"[{name} (StartDialogue)] Is Selected Dialogue DataRunTime Multiple : {IsMultipleDialogue(_selectedDialogueDataRunTime)}");
        ChangeDialogueData();
    }

    public void ContinueDialogue()
    {
        if (!IsDialogueRunning || TransitionManager.Instance.isTrasitioning) return;
        
        _dialogueIndex++;

        if (_dialogueIndex < _currentDialogueData.dialogueLines.Count)
        {
            DisplayDialogue();
        }
        else if (IsMultipleDialogue(_selectedDialogueDataRunTime) && _dialogueDataIndex < _selectedDialogueDataRunTime.dialogueData.Count - 1)
        {
            _dialogueIndex = -1;
            ChangeDialogueData();
        }
        else
        {
            StopDialogue();
        }
    }

    public void SkipDialogue()
    {
        if (!IsDialogueRunning) return;

        if (IsMultipleDialogue(_selectedDialogueDataRunTime) && _dialogueDataIndex < _selectedDialogueDataRunTime.dialogueData.Count - 1)
        {
            ChangeDialogueData();
        }
        else
        {
            StopDialogue();
        }
    }

    public bool IsAllDoneDialogue()
    {
        bool allDoneDialogue = true;

        foreach (var dialogueData in dialogueDataRunTimes)
        {
            if (!dialogueData.isComplete)
                allDoneDialogue = false;
        }
        
        return allDoneDialogue;
    }
    
    public void StopDialogue()
    {
        if (!IsDialogueRunning) return;
        
        Debug.LogWarning($"[{name} (StopDialogue)] Dialogue is stopped!");
        _selectedDialogueDataRunTime.isComplete = true;
        
        TransitionManager.Instance.TransitionDialogueEnvironment("","FadeOut");
        
        IsDialogueRunning = false;
    }
}
