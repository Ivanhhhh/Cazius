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

    [Header("Flying Stats (Scriptable Object)")]
    [SerializeField] private FlyingEnemyStatsSO _flyingStats; // <--- El SO

    [Header("Projectile Settings")]
    [SerializeField] private GameObject _projectilePrefab; 
    [SerializeField] private Transform _muzzlePoint;       
    [SerializeField] private float _projectileSpeed = 20f;

    [Header("Attack Warning VFX")]
    [SerializeField] private GameObject _attackWarningVFXPrefab;
    [SerializeField] private float _warningDelay = 0.15f;
    [SerializeField] private float _warningVFXLifetime = 1f;
    [SerializeField] private bool _parentWarningVFXToMuzzle = true;

    [Header("Patrolling Variables")]
    [SerializeField] private float _patrolSpeed = 3f;
    [SerializeField] private float _patrollingRotationSpeed = 2f;
    [SerializeField] private float _patrolAcceleration = 3f;
    [SerializeField] private float _waypointTolerance = 1.5f;

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

    [Header("Visual Debugging (Rays)")]
    [SerializeField] private bool _showTargetingRay = true;     
    [SerializeField] private bool _showWanderRay = true;        
    [SerializeField] private bool _showIdealMovementRay = true; 
    [SerializeField] private bool _showAvoidanceRay = true;     
    [SerializeField] private bool _showFinalDirectionRay = true;
    [SerializeField] private bool _showVelocityRay = true;     

    [Header("Health System")] 
    [SerializeField] private Enemy_HealthSystem_Base _healthSystem;

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
        _fieldOfView = new Enemy_FieldOfViewBehaviour(_playerTransform, _radiusVision, _objectTransform, _horizontalAngleVision, _verticalAngleVision, _lineOfSightLayerMask, _flyingStats.aimOffset);
        _obstacleBehaviour = new Enemy_ObstacleAvoidanceBehaviour(_objectTransform, _sensorLength, _avoidanceForce, _obstacleMask, _interiorSensorLength, _interiorAvoidanceForce);
        
        _patrolling = new Enemy_FlyingPatrollingBehaviour(_objectTransform, _rb, _flightZone, _obstacleBehaviour, 
        _patrolSpeed, _patrollingRotationSpeed, _patrolAcceleration, _waypointTolerance, _healthSystem);
        
        // Inyectamos "this" al final para que el comportamiento tenga acceso a instanciar proyectiles
        _chasing = new Enemy_FlyingCasterAttackBehaviour(_objectTransform, _rb, _playerTransform, _fieldOfView, _obstacleBehaviour, _flyingStats, this);
    }

    // ==========================================
    // SISTEMA DE PROYECTILES
    // ==========================================
    public void SpawnProjectile(Vector3 direction)
    {
        if (_projectilePrefab == null)
        {
            Debug.LogWarning("Falta asignar el Prefab del Proyectil en el Inspector.");
            return;
        }

        // Si asignaste un cañón, el disparo sale de ahí; si no, sale del centro del enemigo
        Vector3 spawnPosition = (_muzzlePoint != null) ? _muzzlePoint.position : _objectTransform.position;

        // Instanciamos la bala mirando hacia donde va a viajar
        GameObject projectile = Instantiate(_projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));

        // Le damos el empuje físico
        Rigidbody projRb = projectile.GetComponent<Rigidbody>();
        if (projRb != null)
        {
            // Resetear la velocidad por si acaso y aplicar la nueva dirección
            projRb.linearVelocity = Vector3.zero;
            projRb.linearVelocity = direction * _projectileSpeed;
        }
    }

    public void SpawnProjectileWithWarning(Vector3 direction)
    {
        StartCoroutine(SpawnProjectileWithWarningRoutine(direction));
    }

    private IEnumerator SpawnProjectileWithWarningRoutine(Vector3 direction)
    {
        Vector3 spawnPosition = (_muzzlePoint != null) ? _muzzlePoint.position : _objectTransform.position;

        Quaternion spawnRotation = direction.sqrMagnitude > 0.01f
            ? Quaternion.LookRotation(direction)
            : _objectTransform.rotation;

        if (_attackWarningVFXPrefab != null)
        {
            GameObject warningVFX = Instantiate(_attackWarningVFXPrefab, spawnPosition, spawnRotation);

            if (_parentWarningVFXToMuzzle && _muzzlePoint != null)
            {
                warningVFX.transform.SetParent(_muzzlePoint);
                warningVFX.transform.localPosition = Vector3.zero;
                warningVFX.transform.localRotation = Quaternion.identity;
            }

            Destroy(warningVFX, _warningVFXLifetime);
        }

        yield return new WaitForSeconds(_warningDelay);

        SpawnProjectile(direction);
    }
}