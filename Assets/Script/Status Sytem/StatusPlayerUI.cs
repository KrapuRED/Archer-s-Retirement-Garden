using System;
using TMPro;
using UnityEngine;

public class StatusPlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text statusTextUI;
    [SerializeField] private UpgradeStatusType statusTypeUI;

    private void OnEnable()
    {
        GameEvents.OnUpdateStatusCharacter.AddListener(UpdateStatusPlayerUI);
    }

    private void OnDisable()
    {
        GameEvents.OnUpdateStatusCharacter.RemoveListener(UpdateStatusPlayerUI);
    }

    private void UpdateStatusPlayerUI(UpgradeStatusType statusType, float statusValue)
    {
        if (statusType != statusTypeUI) return;
        
        if (statusTypeUI == UpgradeStatusType.CritChance || statusTypeUI == UpgradeStatusType.CritDamage)
        {
            statusTextUI.text = $"{statusValue:F1}%";
        }
        else if (statusType == UpgradeStatusType.AttackInterval)
        {
            statusTextUI.text = $"{statusValue:F1}s";
        }
        else
        {
            statusTextUI.text = $"{statusValue}";
        }
    }
}
