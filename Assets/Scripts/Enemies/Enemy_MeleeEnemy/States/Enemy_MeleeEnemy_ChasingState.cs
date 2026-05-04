using UnityEngine;

public class Enemy_MeleeEnemy_ChasingState : Enemy_MeleeEnemy_Interface_StateMachine
{
    Enemy_MeleeEnemy_StateMachine _stateMachine;
    Enemy_MeleeEnemy_Data _data;
    AngelDemonAnim _angelDemonAnim;

    public Enemy_MeleeEnemy_ChasingState(Enemy_MeleeEnemy_StateMachine stateMachine, Enemy_MeleeEnemy_Data data)
    {
        _stateMachine = stateMachine;
        _data = data;
        _angelDemonAnim = _data.GetComponent<AngelDemonAnim>();
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
        _angelDemonAnim.AttackFalse();
        if (_data._chasing._isNearToAttack)
        {
            //int attackSelction = Random.Range(0,2);
            int attackSelction = 0;
            if (attackSelction == 0)
            {
                Debug.Log("Doing First attack");
                _angelDemonAnim.AttackAnim();
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
