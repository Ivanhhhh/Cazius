using Patterns.Observer.EventManager_Delegates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : IState
{
    FSM<AgentStates> _fsm;

    public PauseState(FSM<AgentStates> fsm)
    {
        _fsm = fsm;
    }

    public void OnEnter()
    {
        EventManager.TriggerEvent(EventsType.Event_PauseGame);
    }

    public void OnExit()
    {
        //Debug.Log("OnExit de Dead");
    }

    public void OnUpdate()
    {
        //Debug.Log("OnUpdate de Dead");
    }

}

