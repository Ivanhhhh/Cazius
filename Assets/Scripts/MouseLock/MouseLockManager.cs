using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class MouseLockManager : MonoBehaviour
{
     void OnEnable()
     {
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, LockMouse);
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, UnLockMouse);
     }

    void OnDisable()
    {
        EventManager.UnsubscribeToEvent(EventsType.Event_PauseGame, LockMouse);
        EventManager.UnsubscribeToEvent(EventsType.Event_PauseGame, UnLockMouse);
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
