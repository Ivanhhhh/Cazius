using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic; // Necesario para el Action

[RequireComponent(typeof(Rigidbody))]
public class Enemy_OrbitEnemyData : MonoBehaviour
{
    [Header("General References")]
    [SerializeField] private Transform _objectTransform;
    [SerializeField] private BoxCollider _flightZone;
    private Rigidbody _rb;
    private Transform _playerTransform;
 
    [Header("Flying Stats (Scriptable Object)")]
    [SerializeField] private FlyingEnemyStatsSO _flyingStats;
 
    [Header("Orbit Attack Settings")]
    [SerializeField] private OrbitManager _orbitManager;
    [SerializeField] private float _bulletSpeed = 25f;
 
    [Header("Spawn Enemies")]
    [SerializeField] private List<GameObject> _enemiesAlive = new List<GameObject>();
 
    [Header("Second Attack Settings")]
    [SerializeField] private float _secondAttackDuration = 5f;
    [SerializeField] private float _speedBoostMultiplier = 2f;
    [SerializeField, Range(0f, 1f)] private float _secondAttackChance = 0.3f;
    [SerializeField] private float _secondAttackCooldown = 8f; // NUEVO: tiempo mínimo entre intentos de segundo ataque
 
    // EVENTO PURO C#
    public event Action OnRequireProjectiles;
    public event Action OnSecondAttackMade;
 
    /// <summary>
    /// Se dispara cada vez que _enemiesAlive pasa de vacia a tener elementos, o viceversa.
    /// El parametro indica si hay al menos un enemigo vivo en la lista.
    /// </summary>
    public event Action<bool> OnEnemiesAliveChanged;
 
    public float SecondAttackDuration => _secondAttackDuration;
    public float SpeedBoostMultiplier => _speedBoostMultiplier;
    public float SecondAttackChance => _secondAttackChance;
    public float SecondAttackCooldown => _secondAttackCooldown; // NUEVO
 
    public List<GameObject> EnemiesAlive => _enemiesAlive;
    public bool HasEnemiesAlive => _enemiesAlive.Count > 0;
 
    // Accesos para el cerebro
    public OrbitManager OrbitManager => _orbitManager;
    public float BulletSpeed => _bulletSpeed;
 
    [Header("Patrullaje y Sensores")]
    [SerializeField] private float _patrolSpeed = 3f;
    [SerializeField] private float _patrollingRotationSpeed = 2f;
    [SerializeField] private float _patrolAcceleration = 3f;
    [SerializeField] private float _waypointTolerance = 1.5f;
    [SerializeField] private float _radiusVision = 15f;
    [SerializeField] private float _horizontalAngleVision = 90f;
    [SerializeField] private float _verticalAngleVision = 180f; 
    [SerializeField] private LayerMask _lineOfSightLayerMask;
    [SerializeField] private float _sensorLength = 4f;
    [SerializeField] private float _avoidanceForce = 15f;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _interiorSensorLength = 1.5f;
    [SerializeField] private float _interiorAvoidanceForce = 5f;
    [SerializeField] private Enemy_HealthSystem_Base _healthSystem;
 
    public Enemy_FlyingPatrollingBehaviour _patrolling { get; private set; }
    public Enemy_FlyingChasingBehaviour _chasing { get; private set; }
    public Enemy_FieldOfViewBehaviour _fieldOfView { get; private set; }
    public Enemy_ObstacleAvoidanceBehaviour _obstacleBehaviour { get; private set; }
 
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false; 
        _rb.constraints = RigidbodyConstraints.FreezeRotation; 
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
        
        _patrolling = new Enemy_FlyingPatrollingBehaviour(_objectTransform, _rb, _flightZone, _obstacleBehaviour, _patrolSpeed, _patrollingRotationSpeed, _patrolAcceleration, _waypointTolerance, _healthSystem);
        
        // Inicializamos el Orbit Behaviour pasándole el SO y este script Data
        _chasing = new Enemy_FlyingOrbitAttackBehaviour(_objectTransform, _rb, _playerTransform, _fieldOfView, _obstacleBehaviour, _flyingStats, this);
    }
 
    // Método seguro para que el Behaviour dispare el evento
    public void InvokeRequireProjectiles()
    {
        OnRequireProjectiles?.Invoke();
    }
 
    public void StartSecondAttackSequence()
    {
        StartCoroutine(SecondAttackDelayRoutine());
    }
 
    private IEnumerator SecondAttackDelayRoutine()
    {
        yield return new WaitForSeconds(_secondAttackDuration);
        OnSecondAttackMade?.Invoke();
    }
 
    public void AddEnemyAlive(GameObject enemy)
    {
        bool hadEnemiesBefore = HasEnemiesAlive;
        _enemiesAlive.Add(enemy);
 
        if (!hadEnemiesBefore)
            OnEnemiesAliveChanged?.Invoke(true);
    }
 
    public void RemoveEnemyAlive(GameObject enemy)
    {
        _enemiesAlive.Remove(enemy);
 
        if (!HasEnemiesAlive)
            OnEnemiesAliveChanged?.Invoke(false);
    }
 
    private void OnDrawGizmosSelected()
    {
        if (_objectTransform == null) _objectTransform = transform;
 
        // Gizmos del Campo de Visión
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
 
        // Gizmo del Rango de Ataque (leyendo desde el ScriptableObject si está asignado)
        if (_flyingStats != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_objectTransform.position, _flyingStats.attackRange);
        }
 
        // Gizmo de Evasión de Obstáculos
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_objectTransform.position, _sensorLength);
 
        // Gizmos de Patrullaje (Solo visibles en modo Play)
        if (Application.isPlaying && _patrolling != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_patrolling.CurrentTarget, 0.5f);
            Gizmos.DrawLine(_objectTransform.position, _patrolling.CurrentTarget);
        }
    }
}
 
