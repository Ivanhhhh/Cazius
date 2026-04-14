using UnityEngine;

public class Enemy_MeleeEnemy_ChasingState : Enemy_MeleeEnemy_Interface_StateMachine
{
    Enemy_MeleeEnemy_StateMachine _stateMachine;
    Enemy_MeleeEnemy_Data _data;

    public Enemy_MeleeEnemy_ChasingState(Enemy_MeleeEnemy_StateMachine stateMachine, Enemy_MeleeEnemy_Data data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }
    public void OnEnter()
    {
        _data._chasing.EnterChase();
    }
    public void OnExit()
    {
        
    }
    public void OnUpdate()
    {
        _data._chasing.Tick();
        if (_data._chasing._isNearToAttack)
        {
            int attackSelction = Random.Range(0,2);
            if (attackSelction == 0)
            {
                Debug.Log("Doing First attack");
                _stateMachine.ChangeState(Enemy_MeleeEnemy_States.DoingFirstAttack);
            }
            else
            {
                Debug.Log("Doing Second attack");
                _stateMachine.ChangeState(Enemy_MeleeEnemy_States.DoingSecondAttack); 

            }
        }
    }
}
