using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM<TKey>
{
    IState _currentState;
    Dictionary<TKey, IState> _allStates = new Dictionary<TKey, IState>(); // TKey es el enum o lo que pongamos 
    //Dictionary<AgentStates,IState> _allStates = new (); Lo mismo

    public void AddState(TKey newState, IState state)
    {
        if (_allStates.ContainsKey(newState)) return;

        _allStates.Add(newState, state);
    }

    public void ChangeState(TKey newState)
    {
        if (_currentState != null) _currentState.OnExit();

        if (_allStates.ContainsKey(newState)) _currentState = _allStates[newState];

        _currentState?.OnEnter();
    }

    public void ArtificialUpdate()
    {
        if (_currentState != null) _currentState.OnUpdate();
    }

}

public enum AgentStates
{
    Pause, Unpause, Quit
}
public enum LizardStates
{
    LPauseState, LUnpauseState, LQuitState
}

