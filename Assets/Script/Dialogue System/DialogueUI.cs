using Febucci.UI;
using UnityEngine;
using TMPro;
using Febucci.UI.Core;


public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text dialogueLine;
 
    [SerializeField] private TypewriterCore typewriter;
    TextAnimatorSettings _settings;
    
    private void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(typewriter, $"Text Animator Player component is null in {gameObject.name}");
        _settings = TextAnimatorSettings.Instance;
        UnityEngine.Assertions.Assert.IsNotNull(_settings, $"Text Animator Settings is null.");
    }
    
    private void OnEnable()
    {
        GameEvents.OnDisplayDialogue.AddListener(UpdateDialogueUI);
    }

    private void OnDisable()
    {
        GameEvents.OnDisplayDialogue.RemoveListener(UpdateDialogueUI);
    }

    private void UpdateDialogueUI(string charName, string line)
    {
        if (this.characterName != null)
            this.characterName.text = charName;
        
        typewriter.ShowText(line);
    }
}
