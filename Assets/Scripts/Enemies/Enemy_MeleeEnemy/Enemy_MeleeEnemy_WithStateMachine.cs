using UnityEngine;

public class Enemy_MeleeEnemy_WithStateMachine : MonoBehaviour
{
   Enemy_MeleeEnemy_StateMachine _stateMachine;
   [SerializeField] private Enemy_MeleeEnemy_Data _enemyData;
    private void Start()
    {
        _stateMachine = new Enemy_MeleeEnemy_StateMachine();
        _stateMachine.AddState(Enemy_MeleeEnemy_States.Patrolling, new Enemy_MeleeEnemy_PatrollingState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_MeleeEnemy_States.Patrolling, new Enemy_MeleeEnemy_ChasingState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_MeleeEnemy_States.Patrolling, new Enemy_MeleeEnemy_InvestigatingState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_MeleeEnemy_States.Patrolling, new Enemy_MeleeEnemy_DoingFirstAttackState(_stateMachine,_enemyData));
        _stateMachine.AddState(Enemy_MeleeEnemy_States.Patrolling, new Enemy_MeleeEnemy_DoingSecondAttackState(_stateMachine,_enemyData));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
