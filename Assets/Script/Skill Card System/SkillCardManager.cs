using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillCardDataRunTime
{
    public string skillCardName;
    public int skillLevel;
    public float currentDuration; 
    public float currentAttackBoost;
    public float currentRadiusExplosion;
    public float currentMaxCooldown;
    public float currentCooldown;
    public int currentMaxTarget;
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
    [SerializeField] private AutoAttackSkill autoShotSkillCard;
    [SerializeField] private SkillCardSO basicArrowSkillCard;
    [SerializeField] private List<SkillCardDataRunTime> listActiveSkillCardData = new();
    [SerializeField] private SkillCardDataRunTime selectedSkillCard;
    
    private SkillCardDataRunTime _basicArrowSkillCard;
    public HashSet<SkillCardSO> OwnedSkillCards { get; private set; } = new();

    public SkillCardDataRunTime SelectedSkillCard => selectedSkillCard;
    
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
        
        SkillCardDataRunTime newSkillCardDataRunTime = new SkillCardDataRunTime
        {
            skillCardName = basicArrowSkillCard.nameSkillCard,
            currentAttackBoost = basicArrowSkillCard.attackBoostSkillCard,
            currentRadiusExplosion = basicArrowSkillCard.explosionData.explosionRadius,
            currentMaxCooldown = basicArrowSkillCard.cooldownSkillCard,
            skillCardUI = basicSkillCardUI,
            skillCardSo = basicArrowSkillCard,
            isActive = true
        };

        _basicArrowSkillCard = newSkillCardDataRunTime;
        listActiveSkillCardData.Add(newSkillCardDataRunTime);
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
        if (OwnedSkillCards.Contains(skillCard))
            return;
        
        SkillCardUI newSkillCard =  Instantiate(prefabSkillCard, cardSkillContiner);
        if (newSkillCard == null)
        {
            Destroy(newSkillCard);
            return;
        }
            
        newSkillCard.name = $"Card Skill - {skillCard.nameSkillCard}";
            
        SkillCardDataRunTime newSkillCardDataRunTime = new SkillCardDataRunTime
        {
            skillCardName = skillCard.nameSkillCard,
            currentAttackBoost = skillCard.attackBoostSkillCard,
            currentRadiusExplosion = skillCard.explosionData.explosionRadius,
            currentMaxCooldown = skillCard.cooldownSkillCard,
            currentDuration = skillCard.durationActiveSkillCard,
            currentMaxTarget = skillCard.targetSkillCard,
            skillCardUI = newSkillCard,
            skillCardSo = skillCard,
            skillLevel = 1,
            isActive = true
        };
        
        if (_basicArrowSkillCard == null && skillCard.nameSkillCard == newSkillCardDataRunTime.skillCardSo.nameSkillCard)
            _basicArrowSkillCard =  newSkillCardDataRunTime;
        
        newSkillCard.InitSkillCard(newSkillCardDataRunTime);
            
        listActiveSkillCardData.Add(newSkillCardDataRunTime);
        OwnedSkillCards.Add(skillCard);
    }

    private void CoolDownSkillCard()
    {
        foreach (var skillData in listActiveSkillCardData)
        {
            if (skillData.isActive)
            {
                if (skillData.skillCardName == autoShotSkillCard.SkillName && BattleManager.Instance.IsBattleActive)
                {
                     AutoUsingSkillCard(autoShotSkillCard.gameObject, skillData);
                }
                continue;
            }

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

    private void AutoUsingSkillCard(GameObject skillInstance, SkillCardDataRunTime targetSkillCardDataRunTime = null)
    {
        var dataToUse = targetSkillCardDataRunTime;
        if (dataToUse == null)
        {
            Debug.LogError($"[{name} - (UsingSkillCard)] No skill data provided!");
            return;
        }
        
        var skillData = listActiveSkillCardData.Find(x => x.skillCardName == dataToUse.skillCardName);
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
        skillData.currentCooldown = skillData.currentMaxCooldown;
    }
    
    public void SelectSkillCard(SkillCardDataRunTime skillDataRunTime)
    {
        var skill = listActiveSkillCardData.Find(x => x.skillCardName == skillDataRunTime.skillCardName);
        if (skill == null)
        {
            Debug.LogError($"[{name} - (UseSkillCard)] NO DATA for {skillDataRunTime.skillCardName}!");
            return;
        }

        if (!skill.isActive) 
            return;
        
        selectedSkillCard = skill;
        Debug.LogWarning($"[{name} - (UseSkillCard)] Select {skillDataRunTime.skillCardName}!");
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
    
    public void UsingSkillCard(GameObject skillInstance, SkillCardDataRunTime targetSkillCardDataRunTime = null)
    {
        var dataToUse = targetSkillCardDataRunTime ?? selectedSkillCard;
        if (dataToUse == null)
        {
            Debug.LogError($"[{name} - (UsingSkillCard)] No skill data provided!");
            return;
        }
        
        var skillData = listActiveSkillCardData.Find(x => x.skillCardName == dataToUse.skillCardName);
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
        skillData.currentCooldown = skillData.currentMaxCooldown;
        
        selectedSkillCard = null;
    }

    public void UpgradeSkillCard(SkillCardSO skillCardSO)
    {
        var data =  listActiveSkillCardData.Find(x => x.skillCardName == skillCardSO.nameSkillCard);
        if (data == null)
            return;

        data.skillLevel++;
        data.currentAttackBoost      = skillCardSO.attackBoostSkillCard;
        data.currentRadiusExplosion  = skillCardSO.explosionData.explosionRadius;
        data.currentMaxCooldown      = skillCardSO.cooldownSkillCard;
        data.currentMaxTarget        = skillCardSO.targetSkillCard;
        data.currentDuration         = skillCardSO.cooldownSkillCard;
        data.isActive = true;
        
        data.skillCardUI.InitSkillCard(data);
    }

    public SkillCardDataRunTime GetActiveSkillCardSo(SkillCardSO skillCardSo)
    {
        var skillData = listActiveSkillCardData.Find(x => x.skillCardName == skillCardSo.nameSkillCard);

        return skillData;
    }
}
