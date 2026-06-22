using UnityEngine;

public class Enemy_OrbitEnemy_WithStateMachine : MonoBehaviour
{
   Enemy_OrbitEnemy_StateMachine _stateMachine;
   [SerializeField] private Enemy_OrbitEnemyData _enemyData;
    private void Start()
    {
        _stateMachine = new Enemy_OrbitEnemy_StateMachine();
        _stateMachine.AddState(Enemy_OrbitEnemy_States.Patrolling, new Enemy_OrbitEnemy_PatrollingState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_OrbitEnemy_States.Chasing, new Enemy_OrbitEnemy_ChasingState(_stateMachine,_enemyData));
        _stateMachine.ChangeState(Enemy_OrbitEnemy_States.Patrolling);

    }

    void Update()
    {
        _stateMachine.ArtificialUpdate();
    }
}
