using UnityEngine;

public class Enemy_FlyingCasterEnemy_PatrollingState : Enemy_Interface_StateMachine
{
    Enemy_FlyingCasterEnemy_StateMachine _stateMachine;
    Enemy_FlyingCasterEnemyData _data;
    
    public Enemy_FlyingCasterEnemy_PatrollingState(Enemy_FlyingCasterEnemy_StateMachine stateMachine, Enemy_FlyingCasterEnemyData data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }

    public void OnEnter()
    {
        Debug.Log("<color=cyan>[ESTADO: Patrullaje]</color> Entrando al estado. Delegando inicio al Behaviour...");
        _data._patrolling.EnterPatrol();  
    }

    public void OnExit()
    {
        Debug.Log("<color=cyan>[ESTADO: Patrullaje]</color> Saliendo del estado. Delegando apagado al Behaviour...");
        _data._patrolling.ExitPatrol();
    }

    public void OnUpdate()
    {
        _data._patrolling.Tick();

        if (_data._fieldOfView.CanseePlayer() || _data._patrolling.TookDamage)
        {
            Debug.LogWarning("<color=cyan>[ESTADO: Patrullaje]</color> ¡JUGADOR DETECTADO! Solicitando cambio a estado CHASING.");
            _stateMachine.ChangeState(Enemy_FlyingCasterEnemy_States.Chasing);
        }
    }
}