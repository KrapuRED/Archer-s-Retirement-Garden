using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillCardData
{
    public string skillCardName;
    public int skillLevel;
    public float currentAttackBoost;
    public float currentRadiusExplosion;
    public float currentMaxCooldown;
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
    [SerializeField] private SkillCardUI basicSkillCardUI;
    [SerializeField] private SkillCardUI prefabSkillCard;
    
    [Header("Skill Cards Configuration")]
    [SerializeField] private SkillCardSO basicArrowSkillCard;
    [SerializeField] private List<SkillCardSO> skillCards = new();
    [SerializeField] private List<SkillCardData> listSkillCardData = new();
    [SerializeField] private SkillCardData selectedSkillCard;
    
    private SkillCardData _basicArrowSkillCard;
    
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
       if (_basicArrowSkillCard != null) return;
        
        SkillCardData newSkillCardData = new SkillCardData
        {
            skillCardName = basicArrowSkillCard.nameSkillCard,
            currentAttackBoost = basicArrowSkillCard.attackBoostSkillCard,
            currentRadiusExplosion = basicArrowSkillCard.explosionData.explosionRadius,
            currentMaxCooldown = basicArrowSkillCard.cooldownSkillCard,
            skillCardUI = basicSkillCardUI,
            skillCardSo = basicArrowSkillCard,
            isActive = true
        };

        _basicArrowSkillCard = newSkillCardData;
        listSkillCardData.Add(newSkillCardData);
        
        /*if (skillCards.Count <= 0) return;
        
        foreach (var skillCard in skillCards)
            InitializeSkillCards(skillCard);*/
    }

    private void Update()
    {
        if (DayCycleManager.Instance.DayCycleType != DayCycleType.Night) return;
        
        if ((selectedSkillCard == null || selectedSkillCard.skillCardSo == null) && _basicArrowSkillCard != null && _basicArrowSkillCard.isActive)
        {
            selectedSkillCard = _basicArrowSkillCard;
        }
        
        CoolDownSkillCard();
    }

    private void InitializeSkillCards(SkillCardSO skillCard)
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
            currentRadiusExplosion = skillCard.explosionData.explosionRadius,
            currentMaxCooldown = skillCard.cooldownSkillCard,
            skillCardUI = newSkillCard,
            skillCardSo = skillCard,
            skillLevel = 1,
            isActive = true
        };
            
        newSkillCard.InitSkillCard(newSkillCardData);
            
        listSkillCardData.Add(newSkillCardData);
    }

    private void CoolDownSkillCard()
    {
        foreach (var skillData in listSkillCardData)
        {
            if (skillData.isActive)
                continue;

            skillData.currentCooldown -= Time.deltaTime;
            
            if (skillData.skillCardUI != null)
                skillData.skillCardUI.UpdateSkillCard(skillData.currentCooldown);
            
            if (skillData.currentCooldown <= 0)
            {
                if (skillData == _basicArrowSkillCard)
                    _basicArrowSkillCard.isActive = true;
                
                skillData.isActive = true;
            }
        }
    }

    public void SelectSkillCard(SkillCardData skillData)
    {
        var skill = listSkillCardData.Find(x => x.skillCardName == skillData.skillCardName);
        if (skill == null)
        {
            Debug.LogError($"[{name} - (UseSkillCard)] NO DATA for {skillData.skillCardName}!");
            return;
        }

        if (!skill.isActive) 
            return;
        
        selectedSkillCard = skill;
        Debug.LogWarning($"[{name} - (UseSkillCard)] Select {skillData.skillCardName}!");
    }

    public void CancelSkillCard()
    {
        Debug.LogWarning($"[{name} - (UseSkillCard)] Cancel {selectedSkillCard.skillCardName}!");
        
        selectedSkillCard = null;
    }

    public void UnlockSkillCard(SkillCardSO skillCardSo)
    {
        InitializeSkillCards(skillCardSo);
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
        skillData.currentCooldown = selectedSkillCard.currentMaxCooldown;
        
        selectedSkillCard = null;
    }

    public void UpgradeSkillCard(SkillCardSO skillCardSO)
    {
        var data =  listSkillCardData.Find(x => x.skillCardName == skillCardSO.nameSkillCard);
        if (data == null)
        {
            
            return;
        }

        data.skillLevel++;
        data.currentAttackBoost      += skillCardSO.attackBoostSkillCard;
        data.currentRadiusExplosion  += skillCardSO.explosionData.explosionRadius;
        data.isActive = true;
        
        data.skillCardUI.InitSkillCard(data);
    }
}
