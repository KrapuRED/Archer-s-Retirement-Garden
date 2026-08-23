using UnityEngine;

public class NormalArrow : Arrow
{
    public override void SpawnArrow(float radiusExplosion = 0)
    {
        Debug.Log($"{name} Spawn Arrow with radius Explosion = {radiusExplosion}");
    }
}
