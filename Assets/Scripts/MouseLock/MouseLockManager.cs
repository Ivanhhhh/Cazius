using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class MouseLockManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, LockMouse);
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, UnLockMouse);

    }

    public void LockMouse(params object[] param)
    {
       Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnLockMouse(params object[] param)
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
