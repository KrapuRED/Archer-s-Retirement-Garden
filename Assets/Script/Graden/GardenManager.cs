using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GardenObjectEffect
{
    public string nameGardenObject;
    public int stackCount;
    public List<GardenObject> instances = new();
}

public class GardenManager : MonoBehaviour
{
    public static GardenManager Instance { get; private set ; }
    
    [SerializeField] private List<GardenObject> gardenObjects = new();

    [Header("References")] 
    [SerializeField] private GardenItemCardHolder gardenItemCardHolder;
    
    private Dictionary<GardenItemSO, GardenObjectEffect> _gardenObjectEffects = new();
    
    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gardenItemCardHolder.Init();
    }

    public void RegisterGardenObject(GardenObject gardenObject)
    {
        var so = gardenObject.GardenItemSo;
        if (!_gardenObjectEffects.TryGetValue(so, out var effect))
        {
            effect = new GardenObjectEffect {nameGardenObject = gardenObject.name};
            _gardenObjectEffects[so] = effect;
        }
        
        effect.stackCount ++;
        effect.instances.Add(gardenObject);
        
        Debug.LogWarning($"[{name} (RegisterGardenObject)] Success Register GardenObject : {gardenObject.name} stackCount: {effect.stackCount}");
        GameEvents.OnChangeStackGardenObject.Invoke(so, effect.stackCount);
    }

    public void UnregisterGardenObject(GardenObject gardenObject)
    {
        var so = gardenObject.GardenItemSo;
        if (!_gardenObjectEffects.TryGetValue(so, out var effect))
        {
            Debug.LogError($"[{name} (UnregisterGardenObject)] the GardenObjectEffects are empty or There are no SO with {so.gardenItemName}");
            return;
        }
        
        effect.stackCount--;
        effect.instances.Remove(gardenObject);
        
        if (effect.stackCount <= 0)
            _gardenObjectEffects.Remove(so);
        
        Debug.LogWarning($"[{name} (RegisterGardenObject)] Success Unregister GardenObject : {gardenObject.name}");
        GameEvents.OnChangeStackGardenObject.Invoke(so, effect.stackCount);
    }
}
