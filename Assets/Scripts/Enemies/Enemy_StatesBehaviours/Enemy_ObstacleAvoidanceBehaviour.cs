using UnityEngine;

public class Enemy_ObstacleAvoidanceBehaviour : MonoBehaviour
{
    private Transform _transform;
    private LayerMask _obstacleMask;
    
    private float _sensorLength;
    private float _avoidanceForce;
    
    private float _originalSensorLength;
    private float _originalAvoidanceForce;

    // Nuevas variables para el modo interior
    private float _interiorSensorLength;
    private float _interiorAvoidanceForce;

    public Enemy_ObstacleAvoidanceBehaviour(
        Transform transform, 
        float sensorLength, 
        float avoidanceForce, 
        LayerMask obstacleMask,
        float interiorSensorLength,
        float interiorAvoidanceForce)
    {
        _transform = transform;
        _sensorLength = sensorLength;
        _avoidanceForce = avoidanceForce;
        _obstacleMask = obstacleMask;

        _originalSensorLength = sensorLength;
        _originalAvoidanceForce = avoidanceForce;

        // Asignamos las variables inyectadas
        _interiorSensorLength = interiorSensorLength;
        _interiorAvoidanceForce = interiorAvoidanceForce;
    }

    public void SetInteriorMode(bool isInside)
    {
        if (isInside)
        {
            _sensorLength = _interiorSensorLength; 
            _avoidanceForce = _interiorAvoidanceForce; 
        }
        else
        {
            _sensorLength = _originalSensorLength;
            _avoidanceForce = _originalAvoidanceForce;
        }
    }

    // EL MOTOR 360
    public Vector3 GetAvoidanceVector()
    {
        Vector3 avoidance = Vector3.zero;

        Collider[] obstacles = Physics.OverlapSphere(_transform.position, _sensorLength, _obstacleMask);

        foreach (Collider obs in obstacles)
        {
            Vector3 closestPointOnObstacle = obs.ClosestPoint(_transform.position);

            Vector3 pushDirection = _transform.position - closestPointOnObstacle;
            float distanceToWall = pushDirection.magnitude;

            // Usamos 0.01f como margen de seguridad matemático puro para no dividir por cero.
            if (distanceToWall > 0.01f && distanceToWall < _sensorLength)
            {
                avoidance += pushDirection.normalized * (_avoidanceForce * (_sensorLength / distanceToWall));
            }
        }

        return avoidance;
    }
}