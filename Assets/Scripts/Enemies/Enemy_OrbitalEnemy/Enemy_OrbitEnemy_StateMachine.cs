using System.Collections.Generic;
using UnityEngine;

public class Enemy_OrbitEnemy_StateMachine : MonoBehaviour
{
    Enemy_Interface_StateMachine _currentState;
    Dictionary<Enemy_OrbitEnemy_States,Enemy_Interface_StateMachine> _allStates = 
    new Dictionary<Enemy_OrbitEnemy_States, Enemy_Interface_StateMachine>();

    public void AddState(Enemy_OrbitEnemy_States newState, Enemy_Interface_StateMachine state)
    {
       if (!_allStates.ContainsKey(newState))
        {
            _allStates.Add(newState,state);
        }
    }
    public void ChangeState(Enemy_OrbitEnemy_States newState)
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
public enum Enemy_OrbitEnemy_States
{
    Patrolling,
    Chasing
}

