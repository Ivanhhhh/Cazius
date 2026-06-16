using UnityEngine;

public class Enemy_FlyingCasterEnemy_WithStateMachine : MonoBehaviour
{
   Enemy_FlyingCasterEnemy_StateMachine _stateMachine;
   [SerializeField] private Enemy_FlyingCasterEnemyData _enemyData;
    private void Start()
    {
        _stateMachine = new Enemy_FlyingCasterEnemy_StateMachine();
        _stateMachine.AddState(Enemy_FlyingCasterEnemy_States.Patrolling, new Enemy_FlyingCasterEnemy_PatrollingState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_FlyingCasterEnemy_States.Chasing, new Enemy_FlyingCasterEnemy_Chasingstate(_stateMachine,_enemyData));
        _stateMachine.ChangeState(Enemy_FlyingCasterEnemy_States.Patrolling);

    }

    void Update()
    {
        _stateMachine.ArtificialUpdate();
    }
}

