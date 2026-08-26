using UnityEngine;

public class MovementCharacter : MonoBehaviour
{
    [SerializeField] private Character ownerCharacter;
    
    public void MoveCharacterToTarget(Vector3 targetPosition)
    {
        if (ownerCharacter is not EnemyCharacter enemyCharacter)
        {
            Debug.LogWarning($"[{name} - (MoveCharacterToTarget)] The Character is not EnemyCharacter");
            return;
        }
        
        Vector3 currentPosition = enemyCharacter.transform.position;
        
        ownerCharacter.transform.position = Vector3.MoveTowards(currentPosition, targetPosition, enemyCharacter.MoveSpeed * Time.deltaTime);
    }
}
