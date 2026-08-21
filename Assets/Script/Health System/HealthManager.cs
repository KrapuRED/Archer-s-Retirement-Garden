using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set ; }

    [Header("Health Configuration")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private HealthUI  healthUI;
    
    private bool _isInitialize;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public void InitializeHealth(float baseMaxHealth)
    {
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
        _isInitialize = true;
        
        healthUI.InitHealthUI(maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            OnTakeHeal(10);
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
            OnTakeDamage(50);
    }

    public void HealthHandeler(float amount)
    {
        maxHealth += amount;
        healthUI.UpdateHealthSlider(maxHealth);

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            healthUI.UpdateHealthUI(currentHealth);
        }
    }

    public void OnTakeHeal(float amountHeal)
    {
        if (!_isInitialize) return;
        
        currentHealth += amountHeal;
        healthUI.UpdateHealthUI(currentHealth);
    }
    
    public void OnTakeDamage(float amountDamage)
    {
        if (!_isInitialize) return;
        
        currentHealth -= amountDamage;
        healthUI.UpdateHealthUI(currentHealth);
    }
}
