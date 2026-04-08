using UnityEngine;
using UnityEngine.AI;

public class Enemy_ChasingBehaviour
{
    private Transform _playerTransform;
    private NavMeshAgent _agent;
    public Enemy_ChasingBehaviour(Transform playerTransform, float objectSpeed)
    {
        _playerTransform = playerTransform;
        _agent.speed = objectSpeed;
    }   
    void Chase()
    {
        _agent.SetDestination(_playerTransform.position);
    }
}

