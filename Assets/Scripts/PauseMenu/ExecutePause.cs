using Patterns.Observer.EventManager_Delegates;
using System.Collections;
using UnityEngine;

public class ExecutePause : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // EventManager.TriggerEvent(EventsType.Event_PauseGame);
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        // EventManager.TriggerEvent(EventsType.Event_PauseGame);
        // }

        EventManager.TriggerEvent(EventsType.Event_PauseGame);

    }

   
}
