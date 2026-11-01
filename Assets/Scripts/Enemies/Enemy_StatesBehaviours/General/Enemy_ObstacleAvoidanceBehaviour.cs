using UnityEngine;

public class Enemy_ObstacleAvoidanceBehaviour
{
    private Transform _transform;
    private float _sensorLength;
    private float _avoidanceForce;
    private LayerMask _obstacleMask;
    private float _interiorSensorLength;
    private float _interiorAvoidanceForce;

    // Buffer reutilizado para OverlapSphereNonAlloc (cero allocs)
    private readonly Collider[] _overlapBuffer = new Collider[32];
    public Vector3 LastAvoidanceVector { get; private set; }
    public float SensorLength => _sensorLength;
    public float InteriorSensorLength => _interiorSensorLength;


    public Enemy_ObstacleAvoidanceBehaviour(Transform transform, float sensorLength, float avoidanceForce,
        LayerMask obstacleMask, float interiorSensorLength, float interiorAvoidanceForce)
    {
        _transform = transform;
        _sensorLength = sensorLength;
        _avoidanceForce = avoidanceForce;
        _obstacleMask = obstacleMask;
        _interiorSensorLength = interiorSensorLength;
        _interiorAvoidanceForce = interiorAvoidanceForce;
    }

    public Vector3 GetAvoidanceVector(Vector3 extraCheckDirection = default)
    {
        Vector3 avoidanceVector = Vector3.zero;
        int hitCount = 0;
        Vector3 origin = _transform.position;

        // === 1. Consulta única en esfera (cubre 3D completo) ===
        int count = Physics.OverlapSphereNonAlloc(origin, _sensorLength, _overlapBuffer, _obstacleMask);

        for (int i = 0; i < count; i++)
        {
            Collider col = _overlapBuffer[i];
            if (col == null) continue;

            // Ignorar el propio collider del enemigo si está en la misma capa
            if (col.transform == _transform || col.transform.IsChildOf(_transform)) continue;

            // Punto más cercano del collider al enemigo
            Vector3 closest = col.ClosestPoint(origin);
            Vector3 toObstacle = closest - origin;
            float distance = toObstacle.magnitude;

            if (distance < 0.001f) continue;  // solapado, no hay dirección

            Vector3 normal = toObstacle / distance;  // "normal" sintética: hacia el obstáculo

            // Repulsión: empuja en dirección OPUESTA al obstáculo
            Vector3 pushAway = -normal;

            // Deslizamiento: perpendicular a la dirección al obstáculo
            Vector3 slideDir = Vector3.Cross(Vector3.up, normal).normalized;
            // (si quieres deslizamiento 3D completo, proyecta sobre el plano tangente)

            float distanceFactor = 1.0f - (distance / _sensorLength);
            Vector3 finalForce = (pushAway + slideDir * 1.2f).normalized;

            avoidanceVector += finalForce * (_avoidanceForce * distanceFactor);
            hitCount++;
        }

        if (hitCount > 0)
            avoidanceVector /= hitCount;

        // === 2. Sensor interior frontal corto (refuerzo de proximidad) ===
        if (Physics.Raycast(origin, _transform.forward,
                            out RaycastHit closeHit, _interiorSensorLength, _obstacleMask))
        {
            avoidanceVector += closeHit.normal * _interiorAvoidanceForce;
        }

        // === 3. Fix 1: dirección real de movimiento ===
        if (extraCheckDirection.sqrMagnitude > 0.01f)
        {
            Vector3 flatDir = new Vector3(extraCheckDirection.x, 0f, extraCheckDirection.z);
            if (flatDir.sqrMagnitude > 0.01f)
            {
                flatDir.Normalize();
                if (Physics.Raycast(origin, flatDir, out RaycastHit moveHit,
                                    _sensorLength, _obstacleMask))
                {
                    float falloff = 1f - (moveHit.distance / _sensorLength);
                    avoidanceVector += moveHit.normal * (_avoidanceForce * falloff);
                }
            }
        }

        LastAvoidanceVector = avoidanceVector;
        return avoidanceVector;
    }
}