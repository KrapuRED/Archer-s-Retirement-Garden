using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Character Data/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public string characterName;
    public string characterDescription;

    [Header("Character Status")] 
    public int spawnCost;
    public float baseMaxHealth;
    public float baseAttack;
    public float baseAttackSpeed;
    public float baseCritRate;
    public float bassCritDamage;
    public int baseDeathReward;
    
    [Header("Character Sprite")]
    public Sprite characterSprite;
    public Character prefabCharacter;
}
