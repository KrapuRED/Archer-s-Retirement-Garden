using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    [SerializeField] SkillCardData skillCardData;
    
    [Header("UI Elements")]
    [SerializeField] TMP_Text skillName;
    [SerializeField] TMP_Text skillDescription;
    [SerializeField] Image skillIcon; 
    
    public void InitSkillCard(SkillCardData skillCard)
    {
        skillCardData = skillCard;
        
        /*skillName.text = skillCardData.skillCardSo.nameSkillCard;
        skillDescription.text = skillCardData.skillCardSo.descriptionSkillCard;*/

    }
    
    public void OnUsingSkillCard() => SkillCardManager.Instance.SelectSkillCard(skillCardData);
}
