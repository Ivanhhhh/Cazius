using UnityEngine;
using UnityEngine.AI;

public class Enemy_FIrstAttackBehaviour
{
    private float _chargeSpeed;
    private float _preparationTime;
    private float _currentPreparationTime;
    private Transform _playerTransform;
    private NavMeshAgent _agent;
    private float _previousSpeed;
    private enum AttackPhase{ Preparing,Charging,Done}
    private AttackPhase _currentPhase = AttackPhase.Preparing;
    public bool IsDone => _currentPhase == AttackPhase.Done;
    public Enemy_FIrstAttackBehaviour(float chargeSpeed,float preparationTime,Transform playerTransform, NavMeshAgent agent)
    {
        _chargeSpeed = chargeSpeed;
        _preparationTime = preparationTime;
        _currentPreparationTime = preparationTime;
        _playerTransform = playerTransform;
        _agent = agent;
        _previousSpeed = _agent.speed;
    }
    void Tick()
    {
        switch (_currentPhase)
        {
            case AttackPhase.Preparing: UpdatePreparing(); break;
            case AttackPhase.Charging: UpdateCharging(); break;
        }
    }
    void UpdatePreparing()
    {
        _currentPreparationTime -= Time.deltaTime;
        if (_currentPreparationTime <= 0)
        {
            Vector3 pointToAttack = _playerTransform.position;
            _agent.speed = _chargeSpeed;
            _agent.SetDestination(pointToAttack);
            _currentPhase = AttackPhase.Charging;
        }
    }
    void UpdateCharging()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _agent.speed = _previousSpeed;
            _currentPhase = AttackPhase.Done;
        }
    }

    void Reset() 
    {
        _currentPreparationTime = _preparationTime;
        _currentPhase = AttackPhase.Preparing;
    }
}
