using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    [SerializeField] SkillCardData skillCardData;
    
    [Header("UI Elements")]
    [SerializeField] TMP_Text skillLevel;
    [SerializeField] TMP_Text skillCooldown;
    [SerializeField] Image skillIcon; 
    
    public void InitSkillCard(SkillCardData skillCard)
    {
        skillCardData = skillCard;

        skillLevel.text = $"Lv.{skillCard.skillLevel}";
        skillCooldown.text = string.Empty;
    }

    public void UpdateSkillCard(float cooldown)
    {
        if (cooldown <= 0)
            skillCooldown.text = string.Empty;
        
        skillCooldown.text = $"{cooldown:00.00}s";
    }
    
    public void OnUsingSkillCard() => SkillCardManager.Instance.SelectSkillCard(skillCardData);
}
