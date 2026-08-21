using System;
using UnityEngine;

[Serializable]
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
        
        [SerializeField] private EnvironmentController environmentController;
        
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
            if (newDayCycleType == DayCycleType.Day)
                dayCount++;
            
            dayCycleType = newDayCycleType;
            environmentController.ApplyEnvironment(dayCycleType);
        }

        public void UpdateCycleManager()
        {
            bool isDayCycle = dayCycleType == DayCycleType.Day;
            if (isDayCycle)
                ChangeDayCycleType(DayCycleType.Night);
            else
            {
                ChangeDayCycleType(DayCycleType.Day);
            }
        }
}
