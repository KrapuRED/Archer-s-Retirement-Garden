using System;
using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterData;
    [SerializeField] protected string characterID;

    [Header("Character System")]
    [SerializeField] protected MovementCharacter movementCharacter;
    [SerializeField] protected Animator animatorCharacter;
    
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
    public CharacterSO CharacterData => characterData;
    public EnemyRunTimeData RunTimeData {get; private set; }

    public bool IsDead { get; protected set; }

    private void Start()
    {
        if (initByStart)
        {
            currentHealth = maxHealth;
            healthUI.InitHealthUI(maxHealth);
        }
    }

    public void InitializeCharacter(string charID, Vector3 targetPosition, EnemyRunTimeData  runTimeData)
    {
        characterID     = charID;
        TargetPosition  = targetPosition;
        RunTimeData     = runTimeData;
        
        maxHealth = currentHealth = runTimeData.enemyHealth;
        
        Debug.Log($"[{name} - (Character Initialized)] Health: {maxHealth} Attack {runTimeData.enemyAttack} Reward {runTimeData.enemyReward}");
        
        healthUI.InitHealthUI(maxHealth);
    }
    
    public virtual void CharacterDead()
    {
        
    }
}
