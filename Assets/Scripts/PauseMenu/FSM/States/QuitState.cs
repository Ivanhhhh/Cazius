using Patterns.Observer.EventManager_Delegates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitState : IState
{
    FSM<AgentStates> _fsm;

    public QuitState(FSM<AgentStates> fsm)
    {
        _fsm = fsm;
    }

    public void OnEnter()
    {
       // EventManager.TriggerEvent(EventsType.Event_ResumeGame);
    }

    public void OnExit()
    {
       
    }

    public void OnUpdate()
    {
    }

}

