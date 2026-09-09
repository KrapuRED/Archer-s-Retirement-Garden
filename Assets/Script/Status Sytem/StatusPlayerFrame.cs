using UnityEngine;

public class StatusPlayerFrame : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private bool _isOpen;

    public void ShowOrHIdeStatusPlayerFrame()
    {
        if (!_isOpen)
        {
            _isOpen = true;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            _isOpen = false;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
