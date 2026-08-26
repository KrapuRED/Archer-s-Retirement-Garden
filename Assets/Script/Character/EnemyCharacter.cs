using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyCharacter : Character, IDamageable
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float waypointThreshold = 0.05f;

    public float MoveSpeed => moveSpeed;
    public float RotationSpeed => rotationSpeed;
    public float WaypointThreshold => waypointThreshold;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name} collided with {collision.gameObject.name}");
        
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
