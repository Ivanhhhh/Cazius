using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_PatrollingBehaviour
{
    private LayerMask _nodes;
    private float _detectionRadius;
    private float _MaximumAmountOfNodes;
    private float _objectSpeed;
    private Queue<Vector3> _nodesToPatrol;
    private Transform _objectTransform;
    
    void FindPatrolNodes()
    {
        float currrentRadius = _detectionRadius;
        Collider[] nodesInRange = new Collider[0];

        while(nodesInRange.Length < _MaximumAmountOfNodes)
        {
            nodesInRange = Physics.OverlapSphere(_objectTransform.position,_detectionRadius,_nodes);
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
        
    }
}
