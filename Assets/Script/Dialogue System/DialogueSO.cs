using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLineData
{
    public string characterName;
    public string dialogueLine;
}

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public List<DialogueLineData> dialogueLines = new ();
}
