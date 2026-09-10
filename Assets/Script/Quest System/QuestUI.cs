using System;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text questDescription;
    
    private void OnEnable()
    {
        GameEvents.OnDisplayQuestDayCount.AddListener(UpdateQuestUI);
        GameEvents.OnDisplayEndlessQuestDayCount.AddListener(UpdateEndlessQuestUI);
    }

    private void OnDisable()
    {
        GameEvents.OnDisplayQuestDayCount.RemoveListener(UpdateQuestUI);
        GameEvents.OnDisplayEndlessQuestDayCount.RemoveListener(UpdateEndlessQuestUI);
    }

    private void UpdateQuestUI(int currentDay,  int currentDayGoal)
    {
        questDescription.text = $"Reach Day {currentDayGoal}! ({currentDay}/{currentDayGoal})";
    }

    private void UpdateEndlessQuestUI(int currentDay)
    {
        questDescription.text = $"Good Luck in Endless Mode! ({currentDay})";
    }
}
