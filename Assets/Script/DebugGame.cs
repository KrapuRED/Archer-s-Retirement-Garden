using UnityEngine;

public class DebugGame : MonoBehaviour
{
    public GameObject debuGameUI;

    private bool _isDebugOpen;

    public void OpenDebugGame()
    {
        if (_isDebugOpen)
        {
            _isDebugOpen = false;
            debuGameUI.SetActive(_isDebugOpen);
        }
        else
        {
            _isDebugOpen = true;
            debuGameUI.SetActive(_isDebugOpen);
        }
    }
}
