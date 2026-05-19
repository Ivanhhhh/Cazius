using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Security;
public class Enemy_PatrollingBehaviour
{
    private LayerMask _nodes;
    private float _detectionRadius;
    private float _patrolSpeed;
    private int _MaximumAmountOfNodes;
    private Queue<Vector3> _nodesToPatrol;
    private Transform _selfObjectTransform;
    private NavMeshAgent _agent;
    
    // --- NUEVAS VARIABLES ---
    private Enemy_HealthSystem _healthSystem; // Referencia al sistema de vida
    public bool TookDamage { get; private set; } // Propiedad pública para que el State la lea

    // Actualizamos el constructor para pedir el Enemy_HealthSystem
    public Enemy_PatrollingBehaviour(LayerMask nodes, float detectionRadius,
    int maximumAmountOfNodes, float patrolSpeed, Transform objectTransform, NavMeshAgent agent, Enemy_HealthSystem healthSystem)
    {
        _nodes = nodes;
        _detectionRadius = detectionRadius;
        _MaximumAmountOfNodes = maximumAmountOfNodes;
        _selfObjectTransform = objectTransform; 
        _agent = agent;
        _patrolSpeed = patrolSpeed;
        _healthSystem = healthSystem; // Asignamos la referencia
        _nodesToPatrol = new Queue<Vector3>();
    }

    public void EnterPatrol()
    {
        TookDamage = false; // Reiniciamos la memoria
        _agent.speed = _patrolSpeed;
        
        // Nos suscribimos al daño
        _healthSystem.OnDamaged += HandleDamageReceived; 
    }

    private void HandleDamageReceived(float currentHealth)
    {
        TookDamage = true;
    }

    public void FindPatrolNodes()
    {
        float currrentRadius = _detectionRadius;
        Collider[] nodesInRange = new Collider[0];

        // NOTA DE SEGURIDAD: Agregué un límite de seguridad al while para evitar un bucle infinito (cuelgue de Unity) 
        // si en el mapa hay menos nodos creados que el "_MaximumAmountOfNodes" que le pides buscar.
        int safetyNet = 0; 

        while(nodesInRange.Length < _MaximumAmountOfNodes && safetyNet < 50)
        {
            nodesInRange = Physics.OverlapSphere(_selfObjectTransform.position, currrentRadius, _nodes);
            if (nodesInRange.Length < _MaximumAmountOfNodes)
            {
                currrentRadius += 3;
            }
            safetyNet++;
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
        _agent.speed = _patrolSpeed;
    }

    public void Tick()
    {
        // Si ya recibimos daño, podemos detener la patrulla inmediatamente
        if (TookDamage) 
        {
            _agent.isStopped = true;
            return; 
        }

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

    public void Reset()
    {
        _nodesToPatrol.Clear();
        TookDamage = false;
        _healthSystem.OnDamaged -= HandleDamageReceived; // Por si acaso
    }
}
