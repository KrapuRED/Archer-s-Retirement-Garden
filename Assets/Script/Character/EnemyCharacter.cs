using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;

public class EnemyCharacter : Character, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float waypointThreshold = 0.05f;

    [Header("Ground Checker")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundCheckMask;
    [SerializeField] private float groundCheckRadius;
    
    [Header("Visual Effects")]
    [SerializeField] private MMFeedbacks deathFeedback;
    
    public float MoveSpeed => characterData.baseSpeed;
    public float RotationSpeed => rotationSpeed;
    public float WaypointThreshold => waypointThreshold;

    public bool IsGrounded {get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        IsGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundCheckMask);
        
        if (collision.gameObject.CompareTag("MainBuilding"))
        {
            DamageController.Instance.OnCalculateDamageToPlayer(this.characterData);
            CharacterDead();
        }
    }
    
    private void SpawnDamageVisualizer(float amount, bool isCritical)
    {
        if (prefabDamageVisualizer == null || damageContainer == null)
        {
            return;
        }
        
        DamageVisualizer dmgVisualizer = Instantiate(prefabDamageVisualizer, damageContainer.position, Quaternion.identity, damageContainer);
        dmgVisualizer.ShowDamageVisualizer(amount, isCritical);
    }
    
    public void TakeDamage(float amountDamage, bool isCritical)
    {
        Debug.Log($"{name} took {amountDamage} damage and isCritical {isCritical}!");
        currentHealth -= amountDamage;
        healthUI.UpdateHealthUI(currentHealth);
        
        if (currentHealth <= 0)
        {
            CharacterDead();
            return;
        }
        
        //Spawn Damage visualizer
        SpawnDamageVisualizer(amountDamage, isCritical);
    }

    public override void CharacterDead()
    {
        Debug.LogWarning($"[{name} (CharacterDead)] This Character is dead");
        
        IsDead = true;
        GameEvents.OnCharacterDeath.Invoke(this);
        CurrencyManager.Instance.AddCurrency(RunTimeData.enemyReward);
        
        deathFeedback?.PlayFeedbacks();
    }
}
