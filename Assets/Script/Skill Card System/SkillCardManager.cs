
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillCardData
{
    public string skillCardName;
    public float currentDown;
    public bool isActive;
}

public class SkillCardManager : MonoBehaviour
{
    public static SkillCardManager Instance { get; private set; }

    [SerializeField] private List<SkillCardSO> skillCards = new();
    [SerializeField] private List<SkillCardData> skillCardData = new();
    [SerializeField] private SkillCardSO selectedSkillCard;
    
    public SkillCardSO SelectedSkillCard => selectedSkillCard;
    
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
        foreach (SkillCardSO skillCard in skillCards)
        {
            SkillCardData newSkillCardData = new SkillCardData
            {
                skillCardName = skillCard.NameSkillCard,
                currentDown = 0,
                isActive = true
            };
            
            skillCardData.Add(newSkillCardData);
        }
    }

    private void CoolDownSkillCard(SkillCardSO skillCardData)
    {
        
    }
    
    public void UseSkillCard()
    {
        
    }
    
}
