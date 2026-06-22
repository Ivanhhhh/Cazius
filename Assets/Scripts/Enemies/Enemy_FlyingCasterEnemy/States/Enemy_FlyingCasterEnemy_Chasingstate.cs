using UnityEngine;

public class Enemy_FlyingCasterEnemy_Chasingstate : Enemy_Interface_StateMachine
{
    Enemy_FlyingCasterEnemy_StateMachine _stateMachine;
    Enemy_FlyingCasterEnemyData _data;
    

    public Enemy_FlyingCasterEnemy_Chasingstate(Enemy_FlyingCasterEnemy_StateMachine stateMachine, Enemy_FlyingCasterEnemyData data)
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

