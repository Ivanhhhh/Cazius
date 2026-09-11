using UnityEngine;

public class Enemy_MeleeEnemy_PatrollingState : Enemy_Interface_StateMachine
{
    Enemy_MeleeEnemy_StateMachine _stateMachine;
    Enemy_MeleeEnemy_Data _data;

    private float _enterTime;
    private bool _patrolStarted = false;

    public Enemy_MeleeEnemy_PatrollingState(Enemy_MeleeEnemy_StateMachine stateMachine, Enemy_MeleeEnemy_Data data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }

    public void OnEnter()
    {
        _enterTime = Time.time;
        _patrolStarted = false;
    }

    public void OnUpdate()
    {
        // Si necesita esperar y aún no ha pasado el tiempo, no hacemos nada
        if (_data._needTimeForFirstReaction && Time.time - _enterTime < _data._timeBeforeReaction)
            return;

        // Si no necesita esperar, o ya pasó el tiempo, iniciamos la patrulla una vez
        if (!_patrolStarted)
        {
            _data._patrolling.FindPatrolNodes();
            _data._patrolling.EnterPatrol();
            _patrolStarted = true;
        }

        // Detección de jugador y cambio a persecución
        if (_data._fieldOfView.CanseePlayer() || _data._patrolling.TookDamage || _data._isAggresiveOnStart)
        {
            Debug.Log("Player Encontrado");
            _stateMachine.ChangeState(Enemy_MeleeEnemy_States.Chasing);
        }

        // Actualizar patrulla
        _data._patrolling.Tick();
    }

    public void OnExit()
    {
        _data._patrolling.Reset();
    }
}
