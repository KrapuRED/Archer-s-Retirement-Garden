using UnityEngine;
using TMPro;
    

public class UpgradeRefreshButton : MonoBehaviour
{
    [SerializeField] private TMP_Text costText;
    [SerializeField] private int currentPrice;
    [SerializeField] private int basePrice;
    [SerializeField] private float increasePrice;

    private bool _isFirstTime = true;
    
    public void DisplayRefreshButton()
    {
        _isFirstTime = true;
        currentPrice = 0;
        costText.text = $"{currentPrice} $";
    }
    
    public void OnClickButton()
    {
        if (!CurrencyManager.Instance.UseCurrency(currentPrice))
            return;
        
        UpgradeCardManager.Instance.OnShowRandomUpgradeCard();
        
        float newPrice = basePrice * (increasePrice  / 100);
        currentPrice += (int)newPrice;
        
        if (_isFirstTime)
        {
            _isFirstTime = false;
            currentPrice = basePrice;
        }
        
        costText.text = $"{currentPrice} $";
    }
}
