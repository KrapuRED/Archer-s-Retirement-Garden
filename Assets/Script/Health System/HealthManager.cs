using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set ; }

    [Header("Health Configuration")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    private bool _isInitliaze;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() => InitliazeHealth();

    private void InitliazeHealth()
    {
        currentHealth = maxHealth;
        _isInitliaze = true;
    }
    
    public void OnTakeHeal(float amountHeal)
    {
        if (!_isInitliaze) return;
        
        currentHealth += amountHeal;
    }
    
    public void OnTakeDamage(float amountDamage)
    {
        if (!_isInitliaze) return;
        
        currentHealth -= amountDamage;
    }
}
