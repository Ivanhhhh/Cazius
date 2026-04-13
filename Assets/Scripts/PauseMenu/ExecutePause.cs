using Patterns.Observer.EventManager_Delegates;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.TriggerEvent(EventsType.Event_PauseGame);
        }


    }
}
