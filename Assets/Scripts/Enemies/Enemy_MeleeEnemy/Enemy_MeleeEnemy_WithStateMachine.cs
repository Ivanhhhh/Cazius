using UnityEngine;

public class Enemy_MeleeEnemy_WithStateMachine : MonoBehaviour
{
   Enemy_MeleeEnemy_StateMachine _stateMachine;
   [SerializeField] private Enemy_MeleeEnemy_Data _enemyData;
    private void Start()
    {
        _stateMachine = new Enemy_MeleeEnemy_StateMachine();
        _stateMachine.AddState(Enemy_MeleeEnemy_States.Patrolling, new Enemy_MeleeEnemy_PatrollingState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_MeleeEnemy_States.Chasing, new Enemy_MeleeEnemy_ChasingState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_MeleeEnemy_States.Investigating, new Enemy_MeleeEnemy_InvestigatingState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_MeleeEnemy_States.DoingFirstAttack, new Enemy_MeleeEnemy_DoingFirstAttackState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_MeleeEnemy_States.DoingSecondAttack, new Enemy_MeleeEnemy_DoingSecondAttackState(_stateMachine,_enemyData));
        _stateMachine.ChangeState(Enemy_MeleeEnemy_States.Patrolling);
    }

    void Update()
    {
        _stateMachine.ArtificialUpdate();
    }
}
