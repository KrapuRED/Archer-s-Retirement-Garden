using System;
using UnityEngine;

[System.Serializable]
public enum GameMode
{
    Story,
    Endless
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameMode gameMode;
    public GameMode GameMode => gameMode;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
