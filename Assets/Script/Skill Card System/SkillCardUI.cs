using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    [SerializeField] SkillCardDataRunTime skillCardDataRunTime;
    
    [Header("UI Elements")]
    [SerializeField] TMP_Text skillNumberIndex;
    [SerializeField] TMP_Text skillLevel;
    [SerializeField] TMP_Text skillCooldown;
    [SerializeField] Image skillIcon;
    [SerializeField] Image skillCooldownImage;
    [SerializeField] private GameObject coolDownVFX;
    [SerializeField] private GameObject unlockVFX;
    
    [Header("VFX Elements")]
    [SerializeField] private MMFeedbacks selectVFX;
    [SerializeField] private MMFeedbacks unselectVFX;
    
    [SerializeField] private bool _isSelect;
    
    public void InitSkillCard(SkillCardDataRunTime skillCard, int numberIndex)
    {
        skillCardDataRunTime = skillCard;
            
        if (skillLevel != null)
            skillLevel.text = $"Lv.{skillCard.skillLevel}";
        
        if (skillNumberIndex != null)
            skillNumberIndex.text = numberIndex.ToString();

        if (skillIcon != null)
            skillIcon.sprite = skillCard.skillCardSo.iconSkillCard;
        
        coolDownVFX.SetActive(false);
        skillCooldown.text = string.Empty;
    }

    public void UpdateSkillCard(SkillCardDataRunTime skillCard)
    {
        skillCardDataRunTime = skillCard;
        
        if (skillCard.isUnlock)
            unlockVFX.SetActive(false);
        
    }

    public void UpdateCooldownSkillCard(float currentCooldown, float maxCooldown)
    {
        if (coolDownVFX == null)
        {
            Debug.LogError($"coolDownVFX missing on {gameObject.name} (path: {GetGameObjectPath(gameObject)})", this);
            return;
        }
        
        float ratio = currentCooldown / maxCooldown;
        Debug.Log($"coolDownVFX ratio: {currentCooldown} / {maxCooldown} {ratio}");
        skillCooldownImage.fillAmount = ratio;
    
        bool onCooldown = currentCooldown > 0;
        coolDownVFX.SetActive(onCooldown);
        skillCooldown.text = onCooldown ? $"{currentCooldown:00.00}s" : string.Empty;
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

    public void OnUsingSkillCard()
    {
        if (!skillCardDataRunTime.isUnlock)
            return;
        
        if (!_isSelect)
        {
            _isSelect = true;
            selectVFX?.PlayFeedbacks();
            SkillCardManager.Instance.CancelSkillCard();
            InputManager.Instance.ChangeCursorTexture(CursorType.Basic);
        }
        
        InputManager.Instance.ChangeCursorTexture(CursorType.Ability);
        SkillCardManager.Instance.SelectSkillCard(skillCardDataRunTime);
    }

    public void UnSelectSkillCard()
    {
        _isSelect = false;
        unselectVFX?.PlayFeedbacks();
        InputManager.Instance.ChangeCursorTexture(CursorType.Basic);
    }
}
