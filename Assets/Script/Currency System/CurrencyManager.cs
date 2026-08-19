using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int starterCurrency;
    [SerializeField] private int currentCurrency;
    
    [Header("References")]
    [SerializeField] private CurrencyUI currencyUI;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() => AddCurrency(starterCurrency);

    public void AddCurrency(int amountCurrency)
    {
        if (amountCurrency <= 0) return;

        currentCurrency += amountCurrency;
        currencyUI.UpdateCurrencyUI(currentCurrency);
    }

    public bool UseCurrency(int amountCurrency)
    {
        if (currentCurrency < amountCurrency)
        {
            Debug.LogWarning($"[{name} (UseCurrency)] NO ENOUGH AMOUNT CURRENCY! currentCurrency: {currentCurrency}");
            return false;
        }
        
        currentCurrency = Mathf.Max(0, currentCurrency - amountCurrency);
        currencyUI.UpdateCurrencyUI(currentCurrency);

        return true;
    }
    
}
