using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dayCardDeck;
    [SerializeField] private GameObject nightCardDeck;
    
    [Header("Action Map Controls")]
    [SerializeField] private string dayActionMap;
    [SerializeField] private string nightActionMap;
    
    public void ApplyEnvironment(DayCycleType dayCycleType)
    {
        if (dayCycleType == DayCycleType.Day)
        {
            nightCardDeck.SetActive(false);
            dayCardDeck.SetActive(true);
            
            InputManager.Instance.PopInputActionMap();
        }
        else if (dayCycleType == DayCycleType.Night)
        {
            
            dayCardDeck.SetActive(false);
            nightCardDeck.SetActive(true);
            
            InputManager.Instance.SwitchInputMap(nightActionMap);
        }
        
        if (BattleManager.Instance != null)
            BattleManager.Instance.HandelBattle(dayCycleType);
    }
}
