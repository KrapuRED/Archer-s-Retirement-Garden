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

    public void HealthHandler(float amount)
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
        
        float totalHeal = maxHealth * (amountHeal / 100);
        currentHealth += totalHeal;
        
        healthUI.UpdateHealthUI(currentHealth);
        Debug.Log($"OnTakeHeal: {amountHeal} total heal {totalHeal}");
    }
    
    public void OnTakeDamage(float amountDamage, bool isCritical)
    {
        if (!_isInitialize) return;
        
        Debug.Log($"OnTakeDamage: {amountDamage} isCritical {isCritical}");
        
        currentHealth -= amountDamage;
        healthUI.UpdateHealthUI(currentHealth);
        
        if (currentHealth <= 0)
            BattleManager.Instance.LoseBattle();
    }
}
