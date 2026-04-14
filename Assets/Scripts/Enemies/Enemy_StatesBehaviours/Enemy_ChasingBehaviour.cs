using UnityEngine;
using UnityEngine.AI;

public class Enemy_ChasingBehaviour
{
    private Transform _playerTransform;
    private Transform _selfTransform;
    private float _attackRadius;
    private float _chaseSpeed;
    private NavMeshAgent _agent;
    public bool _isNearToAttack => Vector3.Distance(_selfTransform.position,_playerTransform.position) <= _attackRadius;
    public Enemy_ChasingBehaviour(Transform playerTransform, float attackRadius, NavMeshAgent agent, Transform selfTransform, float chaseSpeed)
    {
        _playerTransform = playerTransform;
        _selfTransform = selfTransform;
        _attackRadius = attackRadius;
        _agent = agent;
        _chaseSpeed = chaseSpeed;
    }   
    public void EnterChase()
    {
        _agent.speed = _chaseSpeed;
    }
    public void Tick()
    {
        _agent.SetDestination(_playerTransform.position);
    }
}

