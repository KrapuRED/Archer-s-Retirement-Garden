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

    private void Start() => InitializeHealth();

    private void InitializeHealth()
    {
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
        if (amount > 0)
        {
            IncreaseHealth(amount);
        }
        else
        {
            DecreaseHealth(amount);
        }
    }
    
    private void IncreaseHealth(float amount)
    {
        maxHealth += amount;
        healthUI.UpdateHealthSlider(maxHealth);
    }

    private void DecreaseHealth(float amount)
    {
        maxHealth = Mathf.Max(maxHealth - amount, 100);
        
        healthUI.UpdateHealthSlider(maxHealth);

        if (currentHealth >= maxHealth)
        {
            currentHealth = amount;
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
