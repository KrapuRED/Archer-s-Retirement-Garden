using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomizeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private UnityEvent onHoverEnter;
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onHoverExit;

    public bool IsPointerInside { get; private set; }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[{name}] OnPointerEnter");
        onHoverEnter?.Invoke();
        
        IsPointerInside = true;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[{name}] OnPointerClick");
        onClick?.Invoke();
        
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"[{name}] OnPointerExit");
        onHoverExit?.Invoke();
        
        IsPointerInside = false;
    }
}
