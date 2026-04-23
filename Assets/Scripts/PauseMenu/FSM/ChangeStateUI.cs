using UnityEngine;
using Patterns.Observer.EventManager_Delegates;
using System.Collections;

public class ChangeStateUI : MonoBehaviour
{
    public void ResumeGame()
    {
        // _fsm.ChangeState(AgentStates.Unpause);
        EventManager.TriggerEvent(EventsType.Event_ResumeGame);

        print("se hizo");
    }  
}
