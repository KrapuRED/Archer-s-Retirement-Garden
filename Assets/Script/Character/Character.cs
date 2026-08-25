using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{
    public void TakeDamage(float amountDamage)
    {
        Debug.Log($"{name} took {amountDamage} damage");
    }
    
    public virtual void CharacterDead()
    {
        Debug.Log($"{name} is dead");
    }
}
