
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueDataRunTime
{
    public string DialogueName;
    public int dayDialogue;
    public DialogueSO dialogueData;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private List<DialogueDataRunTime> dialogueDataRunTimes = new();
    
    public bool IsDialogueRunning { get; private set; }
    private DialogueDataRunTime _selectedDialogueDataRunTime;
    private int _dialogueIndex;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Start()
    {
        StartDialogue();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ContinueDialogue();
    }

    private void DisplayDialogue()
    {
        var dialogueLineData = _selectedDialogueDataRunTime.dialogueData.dialogueLines[_dialogueIndex];
        Debug.Log($"[{name}  (DisplayDialogue)] {dialogueLineData.characterName} : {dialogueLineData.dialogueLine}");
    }
    
    public void StartDialogue()
    {
        if (IsDialogueRunning) return;
        
        int dayCount = DayCycleManager.Instance.DayCount;
        var dialogueData = dialogueDataRunTimes.Find(x => x.dayDialogue == dayCount);
        if (dialogueData == null)
        {
            Debug.LogWarning($"[{name} (StartDialogue)] There are no dialogue data for {dayCount} day!");
            return;
        }
        
        _selectedDialogueDataRunTime  = dialogueData;
        IsDialogueRunning = true;
        _dialogueIndex = 0;
        
        DisplayDialogue();
    }

    public void ContinueDialogue()
    {
        if (!IsDialogueRunning) return;
        
        _dialogueIndex++;
        
        if (_dialogueIndex >= _selectedDialogueDataRunTime.dialogueData.dialogueLines.Count)
        {
            StopDialogue();
            return;
        }
        
        DisplayDialogue();
    }

    public void SkipDialogue()
    {
        if (!IsDialogueRunning) return;
        
    }

    public void StopDialogue()
    {
        if (!IsDialogueRunning) return;
        
        Debug.LogWarning($"[{name} (StopDialogue)] Dialogue is stopped!");
        
        IsDialogueRunning = false;
    }
}
