using System;
using UnityEngine;

public abstract class Arrow : MonoBehaviour
{
    [SerializeField] protected float arrowVelocity;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rigidbody.linearVelocity = transform.up * -arrowVelocity;
    }

    public abstract void OnSpawnArrow(SkillCardData skillCardData);
}
