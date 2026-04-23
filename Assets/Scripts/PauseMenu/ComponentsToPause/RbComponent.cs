using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class RbComponent : MonoBehaviour,IPausable
{
    private Rigidbody _Rb;
    void Start()
    {
        

        // a ver srp

    }


    void OnEnable()
    {
        _Rb = GetComponent<Rigidbody>();
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, Pause);
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, UnPause);

    }

    void Disable()
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
