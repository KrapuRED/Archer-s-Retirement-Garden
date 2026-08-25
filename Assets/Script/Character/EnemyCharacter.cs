using UnityEngine;

public class EnemyCharacter : Character, IDamageable
{
    public void TakeDamage(float amountDamage)
    {
        Debug.Log($"{name} took {amountDamage} damage"); 
        CharacterDead();
    }

    public override void CharacterDead()
    {
        Debug.LogWarning($"[{name} (CharacterDead)] This Character is dead");
        GameEvents.OnCharacterDeath.Invoke(this);
        Destroy(gameObject);
    }
}
