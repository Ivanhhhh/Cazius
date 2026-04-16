using UnityEngine;
using Patterns.Observer.EventManager_Delegates;

public class ChangeStateUI : MonoBehaviour
{
    FSM<AgentStates> _fsm;

    //[SerializeField] FSM<AgentStates>fsm;

    public void PauseGame()
    {
        //_fsm.ChangeState(AgentStates.Pause);
        EventManager.TriggerEvent(EventsType.Event_PauseGame);
    }


    public void UnPauseGame()
    {
        _fsm.ChangeState(AgentStates.Unpause);
       // EventManager.TriggerEvent(EventsType.Event_ResumeGame);
    }



    public void CanvasDisable()
    {
        //EventManager.TriggerEvent(EventsType.Event_EnableCanvas);
    }
}
