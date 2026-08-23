
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillCardData
{
    public string skillCardName;
    public float currentAttackBoost;
    public int upgradeSkill;
    public int currentPrice;
    public float currentCooldown;
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
    [SerializeField] private List<SkillCardData> listSkillCardData = new();
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
                currentAttackBoost = skillCard.attackBoostSkillCard,
                skillCardUI = newSkillCard,
                skillCardSo = skillCard,
                isActive = true
            };
            
            newSkillCard.InitSkillCard(newSkillCardData);
            
            listSkillCardData.Add(newSkillCardData);
        }
    }

    private void CoolDownSkillCard()
    {
        foreach (var skillData in listSkillCardData)
        {
            if (skillData.isActive)
                continue;

            skillData.currentCooldown -= Time.deltaTime;
            if (skillData.currentCooldown <= 0)
            {
                skillData.isActive = true;
            }
        }
    }

    public void UpgradeSkillCard(SkillCardSO skillCardData)
    {
        /*
         * 1) Update Skill
         * 2) Update price
         */
    }

    public void SelectSkillCard(SkillCardData skillData)
    {
        var skill = listSkillCardData.Find(x => x.skillCardName == skillData.skillCardName);
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
    
    public void UsingSkillCard(GameObject skillInstance)
    {
        var skillData = listSkillCardData.Find(x => x.skillCardName == selectedSkillCard.skillCardName);
        if (skillData == null)
        {
            Debug.LogError($"[{name} - (UsingSkillCard)] There are no {selectedSkillCard.skillCardName}!");
            return;
        }
    
        Debug.LogWarning($"[{name} - (UseSkillCard)] Using SkillCard!");
        
        var skill = skillInstance.GetComponent<Skill>();
        if (skill == null)
        {
            Debug.LogError($"[{name} - (UsingSkillCard)] There are no Script Skill in {skillData.skillCardSo}!");
            Destroy(skillInstance);
            selectedSkillCard = null;
            return;
        }
        
        foreach (var col in skillInstance.GetComponentsInChildren<Collider>())
            col.enabled = true;
        
        skill.UseSkill(skillData);
        skillData.isActive = false;
        skillData.currentCooldown = selectedSkillCard.skillCardSo.cooldownSkillCard;
        
        selectedSkillCard = null;
    }

}
