using UnityEngine;
using UnityEngine.UI;

public class ParallaxEffect : MonoBehaviour
{
    [Tooltip("This Controll the parallax effect speed")]
    [Range(0f, 1f)] [SerializeField] private float parallaxEffectSpeed;
    [SerializeField] private Camera parallaxCamera;
    
    private float _startPositionX, _lengthParallaxEffect;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        
        _startPositionX = transform.position.x;
        _lengthParallaxEffect = _rectTransform.rect.width;
    }

    private void LateUpdate()
    {
        if (parallaxCamera == null)
        {
            Debug.LogError($"[{name} (LateUpdate)] Parallax Camera is NULL!");
            return;
        }
        
        float distance = parallaxCamera.transform.position.x * parallaxEffectSpeed;
        float movement = parallaxCamera.transform.position.x * (1 - parallaxEffectSpeed);
        
        _rectTransform.position = new Vector3(transform.position.x + distance, transform.position.y, transform.position.z);
        
        if (movement > _startPositionX + _lengthParallaxEffect)
            _startPositionX += _lengthParallaxEffect;
        else if  (movement < _startPositionX - _lengthParallaxEffect)
        {
            _startPositionX -= _lengthParallaxEffect;
        }
    }
}
