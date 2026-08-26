using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterData;
    [SerializeField] protected string characterID;

    [Header("Character System")]
    [SerializeField] protected MovementCharacter movementCharacter;
    
    [Header("Health")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected HealthUI healthUI;
    [SerializeField] protected DamageVisualizer prefabDamageVisualizer;
    [SerializeField] protected Transform damageContainer;

    [SerializeField] private bool initByStart;
    
    public string CharacterID => characterID;
    public MovementCharacter MovementCharacter => movementCharacter;
    public Vector3 TargetPosition { get; private set; }

    private void Start()
    {
        if (initByStart)
        {
            currentHealth = maxHealth;
            healthUI.InitHealthUI(maxHealth);
        }
    }

    public void InitializeCharacter(string charID, Vector3 targetPosition)
    {
        characterID = charID;
        TargetPosition = targetPosition;
        
        maxHealth = currentHealth = characterData.baseMaxHealth;
        
        healthUI.InitHealthUI(maxHealth);
    }
    
    public virtual void CharacterDead()
    {
        
    }
}
