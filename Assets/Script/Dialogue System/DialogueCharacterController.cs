using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum CharacterPositon
{
    Left,
    Right
}

public class DialogueCharacterController : MonoBehaviour
{
    [SerializeField] private Transform characterContiner;
    [SerializeField] private List<DialogueCharacter> characterControllers = new List<DialogueCharacter>();

    private DialogueCharacter _currentDialogueCharacter1;
    private DialogueCharacter _currentDialogueCharacter2;
    
    private void Awake()
    {
        characterControllers.Clear();
        characterControllers = characterContiner.GetComponentsInChildren<DialogueCharacter>().ToList();
    }

    private void LeastRecentUseCharacter(string characterName)
    {
       
    }
    
    public void SwapCharacter(string character)
    {
        string characterName = $"Character - {character}";
        
        var characterController = characterControllers.Find(x => x.CharacterName == characterName);
        if (characterController == null)
        {
            Debug.LogError($"[{name} (SwapCharacter)] Faild to Swap Character {character} because it doesn't exist");
            return;
        }
        
        Debug.Log($"[{name} (SwapCharacter)] Swap Character {characterController.CharacterName}");
    }
}
