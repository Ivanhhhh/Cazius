using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Enemy_FlyingCasterEnemyData : MonoBehaviour
{
    [Header("General References")]
    [SerializeField] private Transform _objectTransform;
    [SerializeField] private BoxCollider _flightZone;
    private Rigidbody _rb;
    private Transform _playerTransform;

    [Header("Patrolling Variables")]
    [SerializeField] private float _patrolSpeed = 3f;
    [SerializeField] private float _patrollingRotationSpeed = 2f;
    [SerializeField] private float _patrolAcceleration = 3f;
    [SerializeField] private float _waypointTolerance = 1.5f;

    [Header("Chasing & Attacking Variables")]
    [SerializeField] private float _chaseSpeed = 6f;
    [SerializeField] private float _chasingRotationSpeed = 5f;
    [SerializeField] private float _attackRange = 10f; // Distancia IA: Cuándo se detiene a disparar
    [SerializeField] private float _shootRange = 25f;  // Distancia Arma: Qué tan largo es el raycast letal
    [SerializeField] private float _shootDelay = 1f;
    [SerializeField] private float _shootCooldown = 2.5f;
    [SerializeField] private LayerMask _playerLayer;

    [Header("Chasing - Advanced Movement")]
    [SerializeField] private float _aimOffset = 1f; // Altura a la que apunta (1f = ombligo/pecho, 0.5f = rodillas)
    [SerializeField] private float _hoverHeight = 4f;
    [SerializeField] private float _retreatMargin = 2f; 
    [SerializeField] private float _normalReactionSpeed = 5f; 
    [SerializeField] private float _evasionReactionSpeed = 15f; 
    [SerializeField] private float _heightCorrectionMultiplier = 2f; 
    
    [Header("Chasing - Random Wander")]
    [SerializeField] private float _minWanderTime = 1.5f;
    [SerializeField] private float _maxWanderTime = 3f;
    [Range(0f, 2f)] [SerializeField] private float _wanderStrafeLimit = 1f;
    [Range(0f, 2f)] [SerializeField] private float _wanderForwardLimit = 0.5f;

    [Header("Field of View Variables")]
    [SerializeField] private float _radiusVision = 15f;
    [Range(0, 360)] [SerializeField] private float _horizontalAngleVision = 90f;
    [Range(0, 360)] [SerializeField] private float _verticalAngleVision = 180f; 
    [SerializeField] private LayerMask _lineOfSightLayerMask;

    [Header("Obstacle Avoidance Variables")]
    [SerializeField] private float _sensorLength = 4f;
    [SerializeField] private float _avoidanceForce = 15f;
    [SerializeField] private LayerMask _obstacleMask;
    
    [Header("Obstacle Avoidance - Interior Mode")]
    [SerializeField] private float _interiorSensorLength = 1.5f;
    [SerializeField] private float _interiorAvoidanceForce = 5f;

    [Header("3D Visual Effects (Lasers)")]
    [SerializeField] private GameObject _charging3DLaser;   // Objeto 3D para la mira/carga
    [SerializeField] private GameObject _shoot3DLaser;      // Objeto 3D para el disparo final
    [SerializeField] private float _chargingLaserThickness = 0.02f;
    [SerializeField] private float _shootLaserThickness = 0.15f;
    [SerializeField] private float _shootLaserDuration = 0.08f;

    [Header("Visual Debugging (Rays)")]
    [SerializeField] private bool _showTargetingRay = true;     
    [SerializeField] private bool _showWanderRay = true;        
    [SerializeField] private bool _showIdealMovementRay = true; 
    [SerializeField] private bool _showAvoidanceRay = true;     
    [SerializeField] private bool _showFinalDirectionRay = true;
    [SerializeField] private bool _showVelocityRay = true;     

    [Header("Health System")] 

    [SerializeField] private Enemy_SUPERHEALTHSYSTEM _healthSystem;

    [Header("Behaviours (Read Only)")]
    public Enemy_FlyingPatrollingBehaviour _patrolling { get; private set; }
    public Enemy_FlyingChasingBehaviour _chasing { get; private set; }
    public Enemy_FieldOfViewBehaviour _fieldOfView { get; private set; }
    public Enemy_ObstacleAvoidanceBehaviour _obstacleBehaviour { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false; 
        _rb.isKinematic = false; 
        _rb.constraints = RigidbodyConstraints.FreezeRotation; 
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (_charging3DLaser != null) _charging3DLaser.SetActive(false);
        if (_shoot3DLaser != null) _shoot3DLaser.SetActive(false);

        StartCoroutine(WaitForPlayer());
    }

    private IEnumerator WaitForPlayer()
    {
        while (_playerTransform == null)
        {
            var player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                _playerTransform = player.transform;
                InitializeBehaviours();
                yield break;
            }
            yield return null;
        }
    }

    private void InitializeBehaviours()
    {
        _fieldOfView = new Enemy_FieldOfViewBehaviour(_playerTransform, _radiusVision, _objectTransform, _horizontalAngleVision, _verticalAngleVision, _lineOfSightLayerMask, _aimOffset);
        _obstacleBehaviour = new Enemy_ObstacleAvoidanceBehaviour(_objectTransform, _sensorLength, _avoidanceForce, _obstacleMask, _interiorSensorLength, _interiorAvoidanceForce);
        
        _patrolling = new Enemy_FlyingPatrollingBehaviour(_objectTransform, _rb, _flightZone, _obstacleBehaviour, 
            _patrolSpeed, _patrollingRotationSpeed, _patrolAcceleration, _waypointTolerance,_healthSystem);
        
        // Inyectamos "this" al final para que el comportamiento tenga acceso a los efectos 3D
            _chasing = new Enemy_FlyingChasingBehaviour(_objectTransform, _rb, _playerTransform, _fieldOfView, _obstacleBehaviour, 
            _chaseSpeed, _chasingRotationSpeed, _attackRange, _shootRange, _shootDelay, _shootCooldown, _playerLayer, 
            _hoverHeight, _retreatMargin, _normalReactionSpeed, _evasionReactionSpeed, _heightCorrectionMultiplier,
            _minWanderTime, _maxWanderTime, _wanderStrafeLimit, _wanderForwardLimit,
            _showTargetingRay, _showWanderRay, _showIdealMovementRay, _showAvoidanceRay, _showFinalDirectionRay, _showVelocityRay,
            _aimOffset, // <--- ¡FALTABA ESTO AQUÍ!
            this);
    }

    // ==========================================
    // SISTEMA DE LÁSERES 3D
    // ==========================================

    public void SetCharging3DLaser(bool active, Vector3 start = default, Vector3 end = default)
    {
        if (_charging3DLaser == null) return;

        _charging3DLaser.SetActive(active);

        if (active)
        {
            Transform3DLaser(_charging3DLaser, start, end, _chargingLaserThickness);
        }
    }

    public void Trigger3DShootLaser(Vector3 start, Vector3 end)
    {
        if (_shoot3DLaser != null)
        {
            StartCoroutine(Shoot3DLaserCoroutine(start, end));
        }
    }

    private IEnumerator Shoot3DLaserCoroutine(Vector3 start, Vector3 end)
    {
        _shoot3DLaser.SetActive(true);
        Transform3DLaser(_shoot3DLaser, start, end, _shootLaserThickness);
        
        yield return new WaitForSeconds(_shootLaserDuration);
        
        _shoot3DLaser.SetActive(false);
    }

    /// <summary>
    /// Método Helper que calcula la matemática espacial de la cápsula.
    /// </summary>
    private void Transform3DLaser(GameObject laserObj, Vector3 start, Vector3 end, float thickness)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // La posición del centro sigue siendo exactamente la mitad del camino
        laserObj.transform.position = start + (direction / 2f);

        if (direction.sqrMagnitude > 0.01f)
        {
            laserObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
        }
        
        // ESCALA ESTIRADA AL 100%: Quitamos el "/ 2f" de la distancia
        laserObj.transform.localScale = new Vector3(thickness, distance, thickness);
    }

    // ==========================================
    // DIBUJO DE GIZMOS
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (_objectTransform == null) _objectTransform = transform;

        Gizmos.color = Color.white;
        Vector3 angleLeft = _objectTransform.rotation * Quaternion.Euler(0, -_horizontalAngleVision / 2, 0) * Vector3.forward;
        Vector3 angleRight = _objectTransform.rotation * Quaternion.Euler(0, _horizontalAngleVision / 2, 0) * Vector3.forward;
        Vector3 angleUp = _objectTransform.rotation * Quaternion.Euler(-_verticalAngleVision / 2, 0, 0) * Vector3.forward;
        Vector3 angleDown = _objectTransform.rotation * Quaternion.Euler(_verticalAngleVision / 2, 0, 0) * Vector3.forward;

        Gizmos.DrawLine(_objectTransform.position, _objectTransform.position + angleLeft * _radiusVision);
        Gizmos.DrawLine(_objectTransform.position, _objectTransform.position + angleRight * _radiusVision);
        
        Gizmos.color = new Color(1, 1, 1, 0.4f); 
        Gizmos.DrawLine(_objectTransform.position, _objectTransform.position + angleUp * _radiusVision);
        Gizmos.DrawLine(_objectTransform.position, _objectTransform.position + angleDown * _radiusVision);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_objectTransform.position, _attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_objectTransform.position, _sensorLength);

        if (Application.isPlaying && _patrolling != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_patrolling.CurrentTarget, 0.5f);
            Gizmos.DrawLine(_objectTransform.position, _patrolling.CurrentTarget);
        }
    }
}