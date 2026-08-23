using UnityEngine;

public abstract class Arrow : MonoBehaviour
{
    [SerializeField] protected float arrowVelocity;
    
    public abstract void SpawnArrow(float radiusExplosion = 0);
}
