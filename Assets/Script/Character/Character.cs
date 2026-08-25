using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterData;
    [SerializeField] protected string characterID;

    public string CharacterID => characterID;
    
    public void InitializeCharacter(string charID)
    {
        characterID = charID;
    }
    
    public virtual void CharacterDead()
    {
        
    }
}
