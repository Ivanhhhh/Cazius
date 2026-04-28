using UnityEngine;

public class ResumeGame : MonoBehaviour
{
    public void OnClick()
    {
        ScreenManager.Instance.RemoveLastScreen();
        PauseManager.Instance.Toggle();
    }
}