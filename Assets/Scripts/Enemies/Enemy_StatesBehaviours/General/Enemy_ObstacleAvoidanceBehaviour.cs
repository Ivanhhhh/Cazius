using UnityEngine;

public class Enemy_ObstacleAvoidanceBehaviour : MonoBehaviour
{
    private Transform _transform;
    private float _sensorLength;
    private float _avoidanceForce;
    private LayerMask _obstacleMask;
    private float _interiorSensorLength;
    private float _interiorAvoidanceForce;

    // Ángulos de los "bigotes" (Rayos) que el enemigo usará para palpar el entorno
    // 0 es el centro, negativos son la izquierda, positivos la derecha
    private readonly float[] _rayAngles = { 0f, -25f, 25f, -50f, 50f };

    public Enemy_ObstacleAvoidanceBehaviour(Transform transform, float sensorLength, float avoidanceForce, LayerMask obstacleMask, float interiorSensorLength, float interiorAvoidanceForce)
    {
        _transform = transform;
        _sensorLength = sensorLength;
        _avoidanceForce = avoidanceForce;
        _obstacleMask = obstacleMask;
        _interiorSensorLength = interiorSensorLength;
        _interiorAvoidanceForce = interiorAvoidanceForce;
    }

    public Vector3 GetAvoidanceVector()
    {
        Vector3 avoidanceVector = Vector3.zero;
        int hitCount = 0;

        for (int i = 0; i < _rayAngles.Length; i++)
        {
            // Calculamos la dirección de este rayo específico
            Vector3 rayDirection = Quaternion.Euler(0, _rayAngles[i], 0) * _transform.forward;

            if (Physics.Raycast(_transform.position, rayDirection, out RaycastHit hit, _sensorLength, _obstacleMask))
            {
                hitCount++;

                // 1. FUERZA DE REPULSIÓN BÁSICA (Alejarse de la pared)
                Vector3 pushAway = hit.normal;

                // 2. FUERZA DE DESLIZAMIENTO TANGENCIAL (Magia anti-rebotes)
                // Usamos el Producto Cruzado entre Arriba y la Normal de la pared para conseguir la dirección paralela a la pared.
                Vector3 slideDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;

                // Decidimos hacia qué lado deslizarnos basándonos en qué rayo golpeó el obstáculo
                if (_rayAngles[i] > 0) 
                {
                    // Si golpeó el rayo derecho, nos deslizamos hacia la izquierda
                    slideDirection = -slideDirection; 
                }
                else if (_rayAngles[i] == 0)
                {
                    // Si golpeó el rayo central de lleno, decidimos aleatoriamente o por la rotación actual para no trabarnos
                    float rightDot = Vector3.Dot(_transform.right, hit.normal);
                    if (rightDot > 0) slideDirection = -slideDirection;
                }

                // Mientras más cerca esté de chocar, más fuerte es la evasión
                float distanceFactor = 1.0f - (hit.distance / _sensorLength);
                
                // Combinamos la repulsión y el deslizamiento
                Vector3 finalForce = (pushAway + (slideDirection * 1.5f)).normalized;
                
                avoidanceVector += finalForce * (_avoidanceForce * distanceFactor);
            }
        }

        // Si múltiples rayos detectan paredes, promediamos la dirección de escape
        if (hitCount > 0)
        {
            avoidanceVector /= hitCount;
            
            // Añadimos un pequeño empuje extra de los "sensores interiores" si está a punto de chocar
            if (Physics.Raycast(_transform.position, _transform.forward, out RaycastHit closeHit, _interiorSensorLength, _obstacleMask))
            {
                avoidanceVector += closeHit.normal * _interiorAvoidanceForce;
            }
        }

        return avoidanceVector;
    }
}