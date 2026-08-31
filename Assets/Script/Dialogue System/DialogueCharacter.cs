using System;
using UnityEngine;

public class DialogueCharacter : MonoBehaviour
{
    [SerializeField] private float speedMovement;
    
    private SpriteRenderer _spriteRenderer;
    public string CharacterName { get; private set; }
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        CharacterName = gameObject.name;
    }
    
    public void MovePointPosition(Vector3 position)
    {
        if (position.x > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
        
        transform.position = Vector3.MoveTowards(transform.position, position, 0.1f);
    }
}
