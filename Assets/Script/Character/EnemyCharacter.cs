using System;
using UnityEngine;

public class EnemyCharacter : Character, IDamageable
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float waypointThreshold = 0.05f;

    [Header("Ground Checker")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundCheckMask;
    [SerializeField] private float groundCheckRadius;
    
    public float MoveSpeed => moveSpeed;
    public float RotationSpeed => rotationSpeed;
    public float WaypointThreshold => waypointThreshold;

    public bool IsGrounded {get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        IsGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundCheckMask);
        
        if (collision.gameObject.CompareTag("MainBuilding"))
        {
            Destroy(gameObject);
        }
    }
    
    public void TakeDamage(float amountDamage)
    {
        Debug.Log($"{name} took {amountDamage} damage"); 
        CharacterDead();
    }

    public override void CharacterDead()
    {
        Debug.LogWarning($"[{name} (CharacterDead)] This Character is dead");
        GameEvents.OnCharacterDeath.Invoke(this);
        Destroy(gameObject);
    }
}
