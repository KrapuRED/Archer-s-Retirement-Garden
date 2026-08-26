using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterData;
    [SerializeField] protected string characterID;

    [Header("Character System")]
    [SerializeField] protected MovementCharacter movementCharacter;
    
    public string CharacterID => characterID;
    public MovementCharacter MovementCharacter => movementCharacter;
    public Vector3 TargetPosition { get; private set; } 
    
    public void InitializeCharacter(string charID, Vector3 targetPosition)
    {
        characterID = charID;
        TargetPosition = targetPosition;
    }
    
    public virtual void CharacterDead()
    {
        
    }
}
