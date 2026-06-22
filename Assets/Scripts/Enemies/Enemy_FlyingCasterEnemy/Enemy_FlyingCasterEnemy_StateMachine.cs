using System.Collections.Generic;
using UnityEngine;

public class Enemy_FlyingCasterEnemy_StateMachine : MonoBehaviour
{
    Enemy_Interface_StateMachine _currentState;
    Dictionary<Enemy_FlyingCasterEnemy_States,Enemy_Interface_StateMachine> _allStates = 
    new Dictionary<Enemy_FlyingCasterEnemy_States, Enemy_Interface_StateMachine>();

    public void AddState(Enemy_FlyingCasterEnemy_States newState, Enemy_Interface_StateMachine state)
    {
       if (!_allStates.ContainsKey(newState))
        {
            _allStates.Add(newState,state);
        }
    }
    public void ChangeState(Enemy_FlyingCasterEnemy_States newState)
    {
        _currentState?.OnExit();
        if(_allStates.ContainsKey(newState)) _currentState = _allStates [newState];
        _currentState?.OnEnter();
    }
    public void ArtificialUpdate()
    {
        if(_currentState != null) _currentState.OnUpdate();
    }

}
public enum Enemy_FlyingCasterEnemy_States
{
    Patrolling,
    Chasing
}
