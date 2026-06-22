using UnityEngine;

public class Enemy_MeleeEnemy_DoingFirstAttackState: Enemy_Interface_StateMachine
{
    Enemy_MeleeEnemy_StateMachine _stateMachine;
    Enemy_MeleeEnemy_Data _data;
    
    public Enemy_MeleeEnemy_DoingFirstAttackState(Enemy_MeleeEnemy_StateMachine stateMachine, Enemy_MeleeEnemy_Data data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }
    public void OnEnter()
    {
        
    }
    public void OnExit()
    {
        _data._firstAttack.Reset();
        _data._chasing.ResetAttackCooldown();
        Debug.Log("Changin to Chasing");
    }
    public void OnUpdate()
    {
        _data._firstAttack.Tick();        
        if (_data._firstAttack.IsDone)
        {
            _stateMachine.ChangeState(Enemy_MeleeEnemy_States.Chasing);
        }
    }
}
