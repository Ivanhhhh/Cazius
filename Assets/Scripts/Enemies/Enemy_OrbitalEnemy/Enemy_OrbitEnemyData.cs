using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.VFX; // Necesario para el Action

[RequireComponent(typeof(Rigidbody))]
public class Enemy_OrbitEnemyData : MonoBehaviour
{
    [Header("General References")]
    [SerializeField] private Transform _objectTransform;
    [SerializeField] private BoxCollider _flightZone;
    private Rigidbody _rb;
    private Transform _playerTransform;
    [SerializeField] GameObject _firstPhaseModel;
    [SerializeField] GameObject _secondphaseModel;
    [SerializeField] private AngelEyeBossVisuals _angelBossVisuals;
    [SerializeField] private ThroneBossVisuals _throneBossVisuals;
    [SerializeField] private GameObject _laserBeamEffect;

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

    [Header("Angel Phase Chasing (independiente)")]
    [Tooltip("Si está asignado, se usa su posición como destino. Si no, se usa player + offset.")]
    [SerializeField] public bool _inSecondPhase;
    [SerializeField] private Transform _angelPhaseTargetAnchor;
    [SerializeField] private Vector3 _angelPhaseTargetOffset = new Vector3(3f, 2f, 0f);
    [SerializeField] private Enemy_MeteoriteSpawnZone _enemyMeteoriteSpawnZone;

    [Header("Angel Phase - Configuración de fases")]
    [SerializeField] private float _angelInitialWaitTime = 2f;
    [SerializeField] private float _angelSecondWaitTime = 2f;
    [SerializeField] private float _angelMoveSpeed = 5f;
    [SerializeField] private float _angelLookRotationSpeed = 8f;
    [SerializeField] private float _angelArriveThreshold = 0.4f;
    [SerializeField] private float _angelBrakeSpeed = 6f;
    [SerializeField] private float _angelAimOffset = 1.5f;
    [Header("Angel Phase - Ataque")]
    [SerializeField] private int _meteoriteMultipleAttackCountCount = 4;
    [Header("Angel Phase - Momentos dentro de SecondWait (segundos desde que empieza)")]
    [SerializeField] private float _modelSwapTime = 0f;
    [SerializeField] private float _throneSpawnTime = 0f;
    [SerializeField] private float _laserPlayTime = 1f;

    public float ModelSwapTime => _modelSwapTime;
    public float ThroneSpawnTime => _throneSpawnTime;
    public float LaserPlayTime => _laserPlayTime;

    // EVENTO PURO C#
    public event Action OnRequireProjectiles;
    public event Action OnSecondAttackMade;

    /// <summary>
    /// Se dispara cada vez que _enemiesAlive pasa de vacia a tener elementos, o viceversa.
    /// El parametro indica si hay al menos un enemigo vivo en la lista.
    /// </summary>
    public event Action<bool> OnEnemiesAliveChanged;
    public event Action OnSecondPhaseSpawnRequested;


    public float SecondAttackDuration => _secondAttackDuration;
    public float SpeedBoostMultiplier => _speedBoostMultiplier;
    public float SecondAttackChance => _secondAttackChance;
    public float SecondAttackCooldown => _secondAttackCooldown; // NUEVO

    public List<GameObject> EnemiesAlive => _enemiesAlive;
    public bool HasEnemiesAlive => _enemiesAlive.Count > 0;
    public bool InSecondPhase => _inSecondPhase;
    public int MeteoriteCount => _meteoriteMultipleAttackCountCount;

    public GameObject FirstPhaseModel => _firstPhaseModel;
    public GameObject SecondPhaseModel => _secondphaseModel;
    public AngelEyeBossVisuals AngelBossVisuals => _angelBossVisuals;
    public ThroneBossVisuals ThroneVisuals => _throneBossVisuals;
    public GameObject LaserBeamEffect => _laserBeamEffect;


    // Accesos para el cerebro
    public OrbitManager OrbitManager => _orbitManager;
    public float BulletSpeed => _bulletSpeed;
    public Enemy_MeteoriteSpawnZone  EnemyMeteoriteSpawnZone => _enemyMeteoriteSpawnZone;

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
    public Enemy_OrbitEnemyAngelPhaseChasingBehaviour _angelPhaseChasing { get; private set; } // NUEVO
    public Enemy_FieldOfViewBehaviour _fieldOfView { get; private set; }
    public Enemy_ObstacleAvoidanceBehaviour _obstacleBehaviour { get; private set; }

    // NUEVO: acceso público al health system para que el state pueda leer la vida
    public Enemy_HealthSystem_Base HealthSystem => _healthSystem;

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

        // NUEVO: comportamiento por fases independiente (Angel Phase)
        Vector3 angelTarget = _angelPhaseTargetAnchor != null
            ? _angelPhaseTargetAnchor.position
            : _playerTransform.position + _angelPhaseTargetOffset;

        _angelPhaseChasing = new Enemy_OrbitEnemyAngelPhaseChasingBehaviour(
            _objectTransform,
            _rb,
            _playerTransform,
            _flyingStats,
            angelTarget
            );
        _angelPhaseChasing = new Enemy_OrbitEnemyAngelPhaseAttackBehaviour(
            _objectTransform,
            _rb,
            _playerTransform,
            _flyingStats,
            angelTarget,
            this);

        _angelPhaseChasing.InitialWaitTime = _angelInitialWaitTime;
        _angelPhaseChasing.SecondWaitTime = _angelSecondWaitTime;
        _angelPhaseChasing.MoveSpeed = _angelMoveSpeed;
        _angelPhaseChasing.LookRotationSpeed = _angelLookRotationSpeed;
        _angelPhaseChasing.ArriveThreshold = _angelArriveThreshold;
        _angelPhaseChasing.BrakeSpeed = _angelBrakeSpeed;
        _angelPhaseChasing.AimOffset = _angelAimOffset;
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

        if (_inSecondPhase)
            RequestSecondPhaseSpawn();           // spawner: prefab de fase 2
        else
            OnSecondAttackMade?.Invoke();        // spawner: prefab de fase 1
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
    public void RequestSecondPhaseSpawn()
    {
        OnSecondPhaseSpawnRequested?.Invoke();
    }

    public void FireAllOrbitProjectiles()
    {
        if (_orbitManager == null || _playerTransform == null) return;

        Vector3 target = _playerTransform.position + Vector3.up * _flyingStats.aimOffset;
        _orbitManager.FireAllAsBullets(target, _bulletSpeed);
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

        // Gizmo del destino de Angel Phase
        if (_angelPhaseTargetAnchor != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_angelPhaseTargetAnchor.position, 0.5f);
            Gizmos.DrawLine(_objectTransform.position, _angelPhaseTargetAnchor.position);
        }
        else if (Application.isPlaying && _playerTransform != null)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.5f);
            Vector3 dynamicTarget = _playerTransform.position + _angelPhaseTargetOffset;
            Gizmos.DrawWireSphere(dynamicTarget, 0.5f);
            Gizmos.DrawLine(_objectTransform.position, dynamicTarget);
        }
    }
}