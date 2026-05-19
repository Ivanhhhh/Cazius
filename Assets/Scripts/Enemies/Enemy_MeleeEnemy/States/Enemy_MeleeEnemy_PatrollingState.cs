using UnityEngine;

public class Enemy_MeleeEnemy_PatrollingState : Enemy_MeleeEnemy_Interface_StateMachine
{
    Enemy_MeleeEnemy_StateMachine _stateMachine;
    Enemy_MeleeEnemy_Data _data;
    

    public Enemy_MeleeEnemy_PatrollingState(Enemy_MeleeEnemy_StateMachine stateMachine, Enemy_MeleeEnemy_Data data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }
    public void OnEnter()
    {
        _data._patrolling.FindPatrolNodes();
        _data._patrolling.EnterPatrol();
    }
    public void OnExit()
    {
        _data._patrolling.Reset();
    }
    public void OnUpdate()
    {
        if (_data._fieldOfView.CanseePlayer() || _data._patrolling.TookDamage)
        {
            Debug.Log("Player Encontrado");
            _stateMachine.ChangeState(Enemy_MeleeEnemy_States.Chasing);
        }
        _data._patrolling.Tick();
    }
}
