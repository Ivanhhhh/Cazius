using UnityEngine;

public class Enemy_MeleeEnemy_DoingSecondAttackState : Enemy_MeleeEnemy_Interface_StateMachine
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
        
    }
    public void OnUpdate()
    {
        
    }
}
