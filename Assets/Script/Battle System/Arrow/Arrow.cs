using UnityEngine;

[System.Serializable]
public enum ArrowType
{
    NormalArrow,
    MagicArrow,
    ExplosiveArrow
}

public abstract class Arrow : MonoBehaviour
{
    [SerializeField] protected float arrowVelocity;
    [SerializeField] protected ArrowType arrowType;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rigidbody.linearVelocity = transform.up * -arrowVelocity;
    }

    public abstract void OnSpawnArrow(SkillCardDataRunTime skillCardDataRunTime);
}
