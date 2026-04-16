using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class RbComponent : MonoBehaviour,IPausable
{
    private Rigidbody _Rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _Rb = GetComponent<Rigidbody>();

        // a ver srp
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, Pause);
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, UnPause);



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
