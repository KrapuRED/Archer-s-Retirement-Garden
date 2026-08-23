using UnityEngine;

public class ExplosionArrow : Arrow
{
    public override void SpawnArrow(float radiusExplosion = 0)
    {
        Debug.Log($"{name} Spawn Arrow with radius Explosion = {radiusExplosion}");
    }
}
