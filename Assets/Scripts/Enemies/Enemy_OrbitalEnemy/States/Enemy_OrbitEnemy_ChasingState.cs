using UnityEngine;

public class Enemy_OrbitEnemy_ChasingState : Enemy_Interface_StateMachine
{
    Enemy_OrbitEnemy_StateMachine _stateMachine;
    Enemy_OrbitEnemyData _data;
    

    public Enemy_OrbitEnemy_ChasingState(Enemy_OrbitEnemy_StateMachine stateMachine, Enemy_OrbitEnemyData data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }
    public void OnEnter()
    {
        _data._chasing.EnterChase();
    }
    public void OnExit()
    {
        _data._chasing.ExitChase();
    }
    public void OnUpdate()
    {
        _data._chasing.Tick();
    }
}
