using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class RbComponent : MonoBehaviour
{
    private Rigidbody _Rb;
    void Start()
    {
        _Rb = GetComponent<Rigidbody>();

        // a ver srp
        
    }


    void OnEnable()
    {
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, Pause);
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, UnPause);
    }

    void OnDisable()
    {
        EventManager.UnsubscribeToEvent(EventsType.Event_PauseGame, Pause);
        EventManager.UnsubscribeToEvent(EventsType.Event_ResumeGame, UnPause);

    }


    public void Pause(params  object []_)
    {
        _Rb.isKinematic = true;
    }

    public void UnPause(params object[] _)
    {
        _Rb.isKinematic = false;
    }
}
