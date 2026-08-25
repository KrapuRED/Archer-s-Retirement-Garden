using UnityEngine;

public class EnemyCharacter : Character
{
    public void TakeDamage(float amountDamage)
    {
        Debug.Log($"{name} took {amountDamage} damage"); 
    }
    
    public override void CharacterDead()
    {
        base.CharacterDead();
    }
}
