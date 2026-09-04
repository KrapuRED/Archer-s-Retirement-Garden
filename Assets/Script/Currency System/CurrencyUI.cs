using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;

    public void UpdateCurrencyUI(int amount)
    {
        currencyText.text = $"{amount}";
    }
}
