using System;
using UnityEngine;

public class DialogueEnvironment : MonoBehaviour
{
    public string EnvironmentID { get; private set; }

    public void Init() => EnvironmentID = gameObject.name;
    public void ShowEnvironment() => gameObject.SetActive(true);
    public void HideEnvironment() => gameObject.SetActive(false);
}
