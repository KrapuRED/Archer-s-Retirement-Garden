
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillCardData
{
    public string skillCardName;
    public float currentDown;
    public int upgradeSkill;
    public int currentPrice;
    public bool isActive;
    
    public SkillCardUI skillCardUI;
    public SkillCardSO skillCardSo;
}

public class SkillCardManager : MonoBehaviour
{
    public static SkillCardManager Instance { get; private set; }

    [Header("UI Elements")] 
    [SerializeField] private Transform cardSkillContiner;
    [SerializeField] private SkillCardUI prefabSkillCard;
    
    [SerializeField] private List<SkillCardSO> skillCards = new();
    [SerializeField] private List<SkillCardData> skillCardDatas = new();
    [SerializeField] private SkillCardData selectedSkillCard;
    
    public SkillCardData SelectedSkillCard => selectedSkillCard;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeSkillCards();
    }

    private void InitializeSkillCards()
    {
        foreach (SkillCardSO skillCard in skillCards)
        {
            SkillCardUI newSkillCard =  Instantiate(prefabSkillCard, cardSkillContiner);
            if (newSkillCard == null)
            {
                Destroy(newSkillCard);
                return;
            }
            
            newSkillCard.name = $"Card Skill - {skillCard.nameSkillCard}";
            
            SkillCardData newSkillCardData = new SkillCardData
            {
                skillCardName = skillCard.nameSkillCard,
                currentDown = 0,
                isActive = true,
                skillCardUI = newSkillCard,
                skillCardSo = skillCard
            };
            
            newSkillCard.InitSkillCard(newSkillCardData);
            
            skillCardDatas.Add(newSkillCardData);
        }
    }

    private void CoolDownSkillCard(SkillCardSO skillCardData)
    {
        
    }

    public void UpdateSkillCard(SkillCardSO skillCardData)
    {
        /*
         * 1) Update Skill
         * 2) Update price
         */
    }

    public void SelectSkillCard(SkillCardData skillData)
    {
        var skill = skillCardDatas.Find(x => x.skillCardName == skillData.skillCardName);
        if (skill == null)
        {
            Debug.LogError($"[{name} - (UseSkillCard)] NO DATA for {skillData.skillCardName}!");
            return;
        }
        
        selectedSkillCard = skill;
        Debug.LogWarning($"[{name} - (UseSkillCard)] Select {skillData.skillCardName}!");
    }

    public void CancelSkillCard()
    {
        Debug.LogWarning($"[{name} - (UseSkillCard)] Cancel {selectedSkillCard.skillCardName}!");
        
        selectedSkillCard = null;
    }
    
    public void UsingSkillCard()
    {
        Debug.LogWarning($"[{name} - (UseSkillCard)] Using SkillCard!");
        
        selectedSkillCard = null;
    }

}
