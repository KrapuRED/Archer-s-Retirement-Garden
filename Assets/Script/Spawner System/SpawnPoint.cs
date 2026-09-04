using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnPointName;
    [SerializeField] private bool rotateSprite;
    
    public bool RotateSprite => rotateSprite;
    public string SpawnPointName => spawnPointName;
    
    private void Awake()
    {
        spawnPointName = name;
    }
}
