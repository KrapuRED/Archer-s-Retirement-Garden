using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestRunTimeData
{
    public string questName;
    public int goalDayQuest;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private List<QuestRunTimeData> runTimeDatas = new List<QuestRunTimeData>();

    private int _questIndex;
    private QuestRunTimeData _currentRunTimeData;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnChangeToDayLight.AddListener(UpdateQuest);
    }

    private void OnDisable()
    {
        GameEvents.OnChangeToDayLight.RemoveListener(UpdateQuest);
    }

    private void Start()
    {
        _questIndex = 0;
        _currentRunTimeData = runTimeDatas[_questIndex];
        
        UpdateQuest();
    }

    private bool IsCurrentQuestComplete(int  currentDayCount, int currentGoalQuest)
    {
        return currentDayCount >= currentGoalQuest;
    }
    
    private void UpdateQuest()
    {
        int currentDayCount = DayCycleManager.Instance.DayCount;
        int currentGoalQuest = _currentRunTimeData.goalDayQuest;
        
        GameEvents.OnDisplayQuestDayCount.Invoke(currentDayCount, currentGoalQuest);

        if (IsCurrentQuestComplete(currentDayCount, currentGoalQuest))
        {
            _questIndex++;
            if (_questIndex >= runTimeDatas.Count)
            {
                GameEvents.OnDisplayEndlessQuestDayCount.Invoke(currentDayCount);
                return;
            }
            
            var questData = runTimeDatas[_questIndex];
            if (questData ==  null)
                return;
            
            _currentRunTimeData = runTimeDatas[_questIndex];
            GameEvents.OnDisplayQuestDayCount.Invoke(currentDayCount, _currentRunTimeData.goalDayQuest);
        }
        
    }
}
