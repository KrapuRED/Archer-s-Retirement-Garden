using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    [SerializeField] SkillCardDataRunTime skillCardDataRunTime;
    
    [Header("UI Elements")]
    [SerializeField] TMP_Text skillLevel;
    [SerializeField] TMP_Text skillCooldown;
    [SerializeField] Image skillIcon;
    [SerializeField] private GameObject coolDownVFX;
    
    public void InitSkillCard(SkillCardDataRunTime skillCard)
    {
        skillCardDataRunTime = skillCard;
        
        if (skillLevel != null)
            skillLevel.text = $"Lv.{skillCard.skillLevel}";
        
        coolDownVFX.SetActive(false);
        skillCooldown.text = string.Empty;
    }

    public void UpdateSkillCard(float cooldown)
    {
        if (coolDownVFX == null)
        {
            Debug.LogError($"coolDownVFX missing on {gameObject.name} (path: {GetGameObjectPath(gameObject)})", this);
            return;
        }
    
        bool onCooldown = cooldown > 0;
        coolDownVFX.SetActive(onCooldown);
        skillCooldown.text = onCooldown ? $"{cooldown:00.00}s" : string.Empty;
    }

    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        return path;
    }
    
    public void OnUsingSkillCard() => SkillCardManager.Instance.SelectSkillCard(skillCardDataRunTime);
}
