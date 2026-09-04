using System;
using TMPro;
using UnityEngine;
using Febucci.UI;
using Febucci.UI.Core;
using MoreMountains.Tools;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text dialogueLine;
    [SerializeField] private AudioClip dialogueBlip;
    
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
        typewriter.onCharacterVisible.AddListener(PlayTypeSound);
    }

    private void OnDisable()
    {
        GameEvents.OnDisplayDialogue.RemoveListener(UpdateDialogueUI);
    }

    private void PlayTypeSound(Char character)
    {
        if (Char.IsWhiteSpace(character))
            return;

        MMSoundManagerSoundPlayEvent.Trigger(dialogueBlip, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
    }
    
    private void UpdateDialogueUI(string charName, string line)
    {
        if (this.characterName != null)
            this.characterName.text = charName;
        
        typewriter.ShowText(line);
    }
}
