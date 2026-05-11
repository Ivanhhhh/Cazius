using Patterns.Observer.EventManager_Delegates;

public class UnPauseState : IState
{
    FSM<AgentStates> _fsm;

    public UnPauseState(FSM<AgentStates> fsm)
    {
        _fsm = fsm;
    }

    public void OnEnter()
    {
        EventManager.TriggerEvent(EventsType.Event_ResumeGame);
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
    }

}

