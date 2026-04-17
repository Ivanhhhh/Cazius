using UnityEngine;
using Patterns.Observer.EventManager_Delegates;

public class ChangeStateUI : MonoBehaviour
{
    FSM<AgentStates> _fsm;

   


    public void ResumeGame()
    {
       // _fsm.ChangeState(AgentStates.Unpause);
         EventManager.TriggerEvent(EventsType.Event_ResumeGame);
    }  
}
