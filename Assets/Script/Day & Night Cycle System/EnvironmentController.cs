using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private GameObject nightButton;
    [SerializeField] private GameObject dayCardDeck;
    [SerializeField] private GameObject nightCardDeck;
    [SerializeField] private GameObject basicIndicatorUI;
    [SerializeField] private GameObject gardenToolsUI;
    
    [Header("Action Map Controls")]
    [SerializeField] private string dayActionMap;
    [SerializeField] private string nightActionMap;
    
    public void ApplyEnvironment(DayCycleType dayCycleType)
    {
        if (dayCycleType == DayCycleType.Day)
        {
            nightButton.SetActive(true);
            
            nightCardDeck.SetActive(false);
            basicIndicatorUI.SetActive(false);
            dayCardDeck.SetActive(true);
            gardenToolsUI.SetActive(true);
            
            InputManager.Instance.PopInputActionMap();
        }
        else if (dayCycleType == DayCycleType.Night)
        {
            nightButton.SetActive(false);
            
            dayCardDeck.SetActive(false);
            basicIndicatorUI.SetActive(true);
            nightCardDeck.SetActive(true);
            gardenToolsUI.SetActive(false);
            
            InputManager.Instance.SwitchInputMap(nightActionMap);
        }
        
        if (BattleManager.Instance != null)
            BattleManager.Instance.HandelBattle(dayCycleType);
    }
}
