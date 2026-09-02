using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialPanel : PanelBase
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private RectTransform containerTutorial;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    
    private List<GameObject> _tutorialObjects = new List<GameObject>();
    private int _tutorialIndex;
    private float _slideDistance;
    private Coroutine _slideRoutine;
    
    private void Awake()
    {
        _tutorialObjects.Clear();
        
        foreach (Transform t in containerTutorial)
            _tutorialObjects.Add(t.gameObject);
        
        Debug.Log(_tutorialObjects.Count);

        if (_tutorialObjects.Count > 0)
        {
            RectTransform firstSlide = _tutorialObjects[0].GetComponent<RectTransform>();
            RectTransform secondSlide = _tutorialObjects[1].GetComponent<RectTransform>();
            
            _slideDistance = Mathf.Abs(secondSlide.anchoredPosition.x - firstSlide.anchoredPosition.x);
            
            if (_slideDistance == 0)
            {
                _slideDistance = viewport.rect.width; 
            }
        }
        else
        {
            _slideDistance = viewport.rect.width;
        }
        
        UpdateButtons();
    }

    public override void OpenPanel()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public override void ClosePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void UpdateButtons()
    {
        prevButton.interactable = _tutorialIndex > 0;
        nextButton.interactable = _tutorialIndex < _tutorialObjects.Count - 1;
    }

    private IEnumerator SlideRoutine(float targetX)
    {
        nextButton.interactable = false;
        prevButton.interactable = false;
        
        Vector2 start = containerTutorial.anchoredPosition;
        Vector2 end = new Vector2(targetX, start.y);
        float t = 0f;

        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float eval = ease.Evaluate(t / slideDuration);
            containerTutorial.anchoredPosition = Vector2.LerpUnclamped(start, end, eval);
            yield return null;
        }

        containerTutorial.anchoredPosition = end;
        UpdateButtons();
    }
    
    private void SlideTo(int index)
    {
        if (_slideRoutine != null) StopCoroutine(_slideRoutine);
        float targetXPosition = -index * _slideDistance;
        
        _slideRoutine = StartCoroutine(SlideRoutine(targetXPosition));
        UpdateButtons();
    }
    
    public void NextTutorial()
    {
        if (_tutorialIndex >= _tutorialObjects.Count - 1) return;
        _tutorialIndex++;
        SlideTo(_tutorialIndex);
    }

    public void PrevTutorial()
    {
        if (_tutorialIndex <= 0) return;
        _tutorialIndex--;
        SlideTo(_tutorialIndex);
    }
}
