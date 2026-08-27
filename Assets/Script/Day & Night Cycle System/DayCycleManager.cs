using UnityEngine;
using TMPro;

[System.Serializable]
public enum DayCycleType
{
    Day,
    Night
}

public class DayCycleManager : MonoBehaviour
{
    public static DayCycleManager Instance { get; private set; }
    
        [SerializeField] private DayCycleType dayCycleType;
        [SerializeField] private int dayCount;
        
        [Header("Refence")]
        [SerializeField] private EnvironmentController environmentController;
        [SerializeField] private TMP_Text dayCountText;
        
        public int DayCount => dayCount;
        public DayCycleType DayCycleType => dayCycleType;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
    
            ComponentChecker();
            ChangeDayCycleType(dayCycleType);
            AddDayCount();
        }
        
    
        private void ComponentChecker()
        {
            if (environmentController == null)
            {
                Debug.LogError($"[{name} - ComponentChecker] Environment Controller is NULL");
            }
        }
        
        private void ChangeDayCycleType(DayCycleType newDayCycleType)
        {
            dayCycleType = newDayCycleType;
            environmentController.ApplyEnvironment(dayCycleType);
        }

        public void UpdateCycleManager()
        {
            bool isDayCycle = dayCycleType == DayCycleType.Day;

            if (isDayCycle)
            {
                ChangeDayCycleType(DayCycleType.Night);
            }
            else
            {
                ChangeDayCycleType(DayCycleType.Day);
                GameEvents.OnChangeToDayLight.Invoke();
            }
        }

        public void AddDayCount()
        {
            dayCount++;
            dayCountText.text = $"Day {dayCount:00}";
        }
}
