using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class CurrencyVisualizer : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private RectTransform rectTransform;
    
    [Header("Colors")]
    [SerializeField] private Color moneyIn = Color.green;
    [SerializeField] private Color moneyOut = new Color(1f, 0.25f, 0.1f);

    [Header("Animation")]
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [SerializeField] private float floatDistance;
    
    public IEnumerator PlayAnimate(int amountCurrency, bool useMoney, Transform spawnPosition, Action onComplete)
    {
        currencyText.text = useMoney ? $"-{amountCurrency}" : $"+{amountCurrency}";
        currencyText.color = useMoney ? moneyOut : moneyIn;
        
        rectTransform.position = spawnPosition.position;
        
        Vector3 startPos = spawnPosition.position;
        Vector3 endPos = startPos + (useMoney ? Vector3.up : Vector3.down) * floatDistance;
        
        float t = 0;

        while (t < lifetime)
        {
            t += Time.deltaTime;
            float normalizedTime = t / lifetime;
            
            transform.position = Vector3.Lerp(startPos, endPos, moveCurve.Evaluate(normalizedTime));
            yield return null;
        }
        
        transform.position = endPos;
        onComplete?.Invoke();
    }
}
