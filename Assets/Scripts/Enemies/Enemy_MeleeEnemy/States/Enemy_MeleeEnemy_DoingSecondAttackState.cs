using NUnit.Framework.Interfaces;
using UnityEngine;

public class Enemy_MeleeEnemy_DoingSecondAttackState : Enemy_Interface_StateMachine
{
    Enemy_MeleeEnemy_StateMachine _stateMachine;
    Enemy_MeleeEnemy_Data _data;

    public Enemy_MeleeEnemy_DoingSecondAttackState(Enemy_MeleeEnemy_StateMachine stateMachine, Enemy_MeleeEnemy_Data data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }
    public void OnEnter()
    {

    }
    public void OnExit()
    {
            _data._secondAttack.Reset();
            Debug.Log("Changin to Chasing");
    }
    public void OnUpdate()
    {
        _data._secondAttack.Tick();
        if (_data._secondAttack.IsDone)
        {
            _stateMachine.ChangeState(Enemy_MeleeEnemy_States.Chasing);
        }
    }
}
