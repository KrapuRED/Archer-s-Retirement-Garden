using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider outerSlider;
    [SerializeField] private Slider innerSlider;
    [SerializeField] private float speedSlider;

    private float _prevHealth;
    private bool _isDamage;
    
    public void InitHealthUI(float maxHealth)
    {
        outerSlider.maxValue = maxHealth;
        innerSlider.maxValue = maxHealth;
        
        outerSlider.value = maxHealth;
        innerSlider.value = maxHealth;
        
        _prevHealth = maxHealth;
    }

    public void UpdateHealthSlider(float amount)
    {
        outerSlider.maxValue = amount;
        innerSlider.maxValue = amount;
    }
    
    public void UpdateHealthUI(float currentHealth)
    {
        if (Mathf.Approximately(currentHealth, _prevHealth))
            return;
        
        if (currentHealth < _prevHealth)
        {
            outerSlider.value = currentHealth;
            _isDamage = true;
        }
        else
        {
            innerSlider.value = currentHealth;
            _isDamage = false;
        }
        
        _prevHealth = currentHealth;
    }

    private void Update()
    {
        if (_isDamage)
        {
            if (!Mathf.Approximately(innerSlider.value, outerSlider.value))
                innerSlider.value = Mathf.MoveTowards(innerSlider.value, outerSlider.value, speedSlider * Time.deltaTime);
        }
        else
        {
            if (!Mathf.Approximately(innerSlider.value, outerSlider.value))
                outerSlider.value = Mathf.MoveTowards(outerSlider.value, innerSlider.value, speedSlider * Time.deltaTime);
        }
    }
}
