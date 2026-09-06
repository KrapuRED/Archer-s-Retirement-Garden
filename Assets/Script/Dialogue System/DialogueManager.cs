using System;
using System.Collections;
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
    private bool _isOpeningTutorial;
    
    private Coroutine _dialogueCoroutine;
    
    public bool IsDialogueRunning { get; private set; }
    public bool IsSkipDialogue { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region Event System
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
    #endregion
    
    private void Start()
    {
        if (dialogueStart)
            StartDialogue();
    }

    private void DisplayDialogue()
    {
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
        dialogueCharacterController.ClearCharacters();
        IsSkipDialogue = false;
        
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

        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
            _dialogueCoroutine = null;
        }
     
        _dialogueCoroutine = StartCoroutine(WaitAndStarDialogue());
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

        if (IsSkipDialogue) return;
        
        IsSkipDialogue = true;
        
        if (IsMultipleDialogue(_selectedDialogueDataRunTime) && _dialogueDataIndex < _selectedDialogueDataRunTime.dialogueData.Count - 1)
        {
            _dialogueIndex = -1;
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

    private bool IsOpeningDialogueDone()
    {
        var dialogueData = dialogueDataRunTimes[0];
        return dialogueData.isComplete; 
    }
    
    public void StopDialogue()
    {
        if (!IsDialogueRunning) return;

        _selectedDialogueDataRunTime.isComplete = true;
        
        IsDialogueRunning = false;
        IsSkipDialogue = false;
        
        TransitionManager.Instance.TransitionDialogueEnvironment("","FadeOut");
        
        if (IsOpeningDialogueDone() && !_isOpeningTutorial)
        {
            _isOpeningTutorial = true;
            
            StartCoroutine(WaitAndShowTutorial());
        }
    }

    private IEnumerator WaitAndShowTutorial()
    {
        if (TransitionManager.Instance != null && TransitionManager.Instance.isTrasitioning)
        {
            yield return new WaitUntil(() => !TransitionManager.Instance.isTrasitioning);
        }
        
        GameEvents.OnRequestOpenPanel.Invoke(PanelType.Tutorial);
    }

    private IEnumerator WaitAndStarDialogue()
    {
        if (TransitionManager.Instance != null && TransitionManager.Instance.isTrasitioning)
        {
            yield return new WaitWhile(() => !TransitionManager.Instance.isTrasitioning);
        }
        
        if (IsDialogueRunning) yield break;
        
        int dayCount = DayCycleManager.Instance.DayCount;
        var dialogueData = dialogueDataRunTimes.Find(x => x.dayDialogue == dayCount);
        
        if (dialogueData == null)
        {
            Debug.LogWarning($"[{name} (StartDialogue)] There are no dialogue data for {dayCount} day!");
            yield break;
        }

        if (dialogueData.isComplete)
        {
            Debug.LogWarning($"[{name} (StartDialogue)] This dialogueData is complete! {dialogueData.dialogueName}");
            yield break;
        }
        
        IsDialogueRunning = true;
        _selectedDialogueDataRunTime  = dialogueData;
        _dialogueIndex = -1;
        _dialogueDataIndex = -1;

        ChangeDialogueData();
    }
}
