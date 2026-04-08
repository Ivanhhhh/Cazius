using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
public class Enemy_PatrollingBehaviour
{
    private LayerMask _nodes;
    private float _detectionRadius;
    private float _MaximumAmountOfNodes;
    private Queue<Vector3> _nodesToPatrol;
    private Transform _selfObjectTransform;
    private NavMeshAgent _agent;
    
    public Enemy_PatrollingBehaviour(LayerMask nodes,float detectionRadius,
    float maximumAmountOfNodes,float objectSpeed,Transform objectTransform,NavMeshAgent agent)
    {
        _nodes = nodes;
        _detectionRadius = detectionRadius;
        _MaximumAmountOfNodes = maximumAmountOfNodes;
        _selfObjectTransform = objectTransform; 
        _agent = agent;
        _agent.speed = objectSpeed;
        _nodesToPatrol = new Queue<Vector3>();
    }
    void FindPatrolNodes()
    {
        float currrentRadius = _detectionRadius;
        Collider[] nodesInRange = new Collider[0];

        while(nodesInRange.Length < _MaximumAmountOfNodes)
        {
            nodesInRange = Physics.OverlapSphere(_selfObjectTransform.position,_detectionRadius,_nodes);
            if (nodesInRange.Length < _MaximumAmountOfNodes)
            {
                currrentRadius += 3;
            }
        }
         if (nodesInRange.Length == 0)
        {
            Debug.LogWarning("No se encontraron waypoints ni con el radio máximo");
            return;
        }
        foreach (Collider node in nodesInRange)
        {
            _nodesToPatrol.Enqueue(node.transform.position);
        }
    }
    void MoveThroughNodes()
    {
        if (_nodesToPatrol.Count == 0) return;
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            GoToNextNode();
        }
    }
    void GoToNextNode()
    {
        if (_nodesToPatrol.Count == 0) return;
        Vector3 nextNode = _nodesToPatrol.Dequeue();
        _agent.SetDestination(nextNode);
        _nodesToPatrol.Enqueue(nextNode);
    }
}
