using UnityEngine;

public class Enemy_FlyingPatrollingBehaviour
{
    private Transform _transform;
    private Rigidbody _rb;
    private BoxCollider _flightZone;
    
    private Enemy_ObstacleAvoidanceBehaviour _avoidanceHelper;

    private float _patrolSpeed;
    private float _rotationSpeed;
    private float _patrolAcceleration;
    private float _waypointTolerance;
    private Vector3 _currentTarget;

    public Vector3 CurrentTarget => _currentTarget;

    private Enemy_SUPERHEALTHSYSTEM _healthSystem; // Referencia al sistema de vida
    public bool TookDamage { get; private set; } // Propiedad pública para que el State la lea

    public Enemy_FlyingPatrollingBehaviour(
        Transform enemyTransform, 
        Rigidbody enemyRb, 
        BoxCollider flightZone, 
        Enemy_ObstacleAvoidanceBehaviour avoidanceHelper, 
        float patrolSpeed, 
        float rotationSpeed,
        float patrolAcceleration,
        float waypointTolerance,
        Enemy_SUPERHEALTHSYSTEM healthSystem)
    {
        _transform = enemyTransform;
        _rb = enemyRb;
        _flightZone = flightZone;
        _avoidanceHelper = avoidanceHelper;
        _patrolSpeed = patrolSpeed;
        _rotationSpeed = rotationSpeed;
        _patrolAcceleration = patrolAcceleration;
        _waypointTolerance = waypointTolerance;
        _healthSystem = healthSystem;
    }

    public void EnterPatrol()
    {
        Debug.Log("[COMPORTAMIENTO: Patrullar] Iniciando patrullaje aéreo sin gravedad.");
        _rb.useGravity = false;
        PickNewTarget();
        _healthSystem.OnDamaged += HandleDamageReceived; 
    }

    private void HandleDamageReceived(float currentHealth)
    {
        TookDamage = true;
    }

    public void Tick()
    {
        Vector3 directionToTarget = (_currentTarget - _transform.position).normalized;
        Vector3 avoidanceDirection = _avoidanceHelper.GetAvoidanceVector();
        
        Vector3 finalDirection = (directionToTarget + avoidanceDirection).normalized;

        // Ahora usa _patrolAcceleration en lugar del 3f hardcodeado
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, finalDirection * _patrolSpeed, Time.fixedDeltaTime * _patrolAcceleration);

        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_rb.linearVelocity.normalized);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
        }

        if (Vector3.Distance(_transform.position, _currentTarget) < _waypointTolerance)
        {
            Debug.Log($"[COMPORTAMIENTO: Patrullar] Nodo alcanzado exitosamente en {_currentTarget}.");
            PickNewTarget();
        }
    }

    public void ExitPatrol()
    {
        Debug.Log("[COMPORTAMIENTO: Patrullar] Finalizando patrullaje. Frenando al enemigo.");
        _rb.linearVelocity = Vector3.zero;
    }

    private void PickNewTarget()
    {
        if (_flightZone == null) 
        {
            Debug.LogError("[COMPORTAMIENTO: Patrullar] ERROR: No hay FlightZone asignada.");
            return;
        }

        Bounds bounds = _flightZone.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        _currentTarget = new Vector3(randomX, randomY, randomZ);
        Debug.Log($"[COMPORTAMIENTO: Patrullar] Generando nuevo nodo aleatorio -> {_currentTarget}");
    }
}