using UnityEngine;
using TMPro;
using System.Collections;

public class DamageVisualizer : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
    
    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color critColor = new Color(1f, 0.25f, 0.1f);

    [Header("Animation")]
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float floatDistance = 1.5f;
    [SerializeField] private float critScaleMultiplier = 1.4f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0.6f, 1, 1, 0);
    
    public void ShowDamageVisualizer(float damage, bool isCritHit)
    {
        damageText.text = $"{Mathf.RoundToInt(damage)}";
        damageText.color = isCritHit ? critColor : normalColor;

        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * floatDistance;
        float t = 0;

        while (t < lifetime)
        {
            t += Time.deltaTime;
            float normalizedTime = t / lifetime;
            
            transform.position = Vector3.Lerp(startPos, endPos, moveCurve.Evaluate(normalizedTime));
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
