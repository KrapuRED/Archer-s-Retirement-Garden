using System;

#region Configuration Custom Events

public class CustomEvents
{
    private event Action Action = delegate { };

    public void Invoke()
    {
        Action?.Invoke();
    }
    
    public void AddListener(Action listener) => Action += listener;
    public void RemoveListener(Action listener) => Action -= listener;
}

public class CustomEvents<T>
{
    private event Action<T> Action = delegate { };

    public void Invoke(T arg)
    {
        Action?.Invoke(arg);
    }
    
    public void AddListener(Action<T> listener) => Action += listener;
    public void RemoveListener(Action<T> listener) => Action -= listener;
}

public class CustomEvents<T1, T2>
{
    private event Action<T1, T2> Action = delegate { };

    public void Invoke(T1 arg1, T2 arg2)
    {
        Action?.Invoke(arg1, arg2);
    }
    
    public void AddListener(Action<T1, T2> listener) => Action += listener;
    public void RemoveListener(Action<T1, T2> listener) => Action -= listener;
}

public class CustomEvents<T1, T2, T3>
{
    private event Action<T1, T2, T3> Action = delegate { };

    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        Action?.Invoke(arg1, arg2, arg3);
    }
    
    public void AddListener(Action<T1, T2, T3> listener) => Action += listener;
    public void RemoveListener(Action<T1, T2, T3> listener) => Action -= listener;
}
#endregion

public static class GameEvents
{
    // # ================================ CHARACTER SYSTEM ================================ #
    public static readonly CustomEvents<Character> OnCharacterDeath = new ();
    
    // # ================================ GARDEN SYSTEM ================================ #
    public static readonly CustomEvents<GardenItemSO, int> OnChangeStackGardenObject = new();
    
    // # ================================ INPUT SYSTEM ================================ #
    public static readonly CustomEvents OnActionMapChange = new();
    public static readonly CustomEvents<GardenItemCardData> OnCarryObject = new();
    
    // # ================================ PAUSE SYSTEM ================================ #
    public static readonly CustomEvents OnChangeToDayLight = new();
    
    // # ================================ PAUSE SYSTEM ================================ #
    public static readonly CustomEvents OnPauseGame  = new();
    public static readonly CustomEvents OnResumeGame = new();
    
    // # ================================ UI ================================ #
    public static readonly CustomEvents<GardenItemCardData> OnShowDetailGardenItem = new();
    public static readonly CustomEvents OnHideDetailGardenItem = new();
    public static readonly CustomEvents<int> OnUpdateCurrencyUI = new();
    public static readonly CustomEvents<PanelType> OnRequestOpenPanel = new();
    public static readonly CustomEvents<PanelType> OnRequestClosePanel = new();
}
