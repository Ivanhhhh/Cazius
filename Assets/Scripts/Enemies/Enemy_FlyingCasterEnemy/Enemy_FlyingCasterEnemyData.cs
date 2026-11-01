using UnityEngine;
using System.Collections;
using FactoryPool;
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
    [SerializeField] private string _projectilePoolId = "Bullet_Caster"; // <-- NUEVO
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
    [Range(0, 360)][SerializeField] private float _horizontalAngleVision = 90f;
    [Range(0, 360)][SerializeField] private float _verticalAngleVision = 180f;
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
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.ScannerSFX, transform.position);

        // 1. Pedir la bala al pool
        Bullet projectile = ObjectFactory.Instance.Get<Bullet>(_projectilePoolId);
        if (projectile == null)
        {
            Debug.LogWarning($"No se encontró pool con id: {_projectilePoolId}");
            return;
        }

        // 2. Posicionar y rotar
        Vector3 spawnPosition = (_muzzlePoint != null) ? _muzzlePoint.position : _objectTransform.position;
        projectile.transform.SetPositionAndRotation(
            spawnPosition,
            Quaternion.LookRotation(direction)
        );

        // 3. Decirle a la bala hacia dónde va y con qué fuerza (una sola llamada)
        projectile.Disparar(direction, _projectileSpeed);
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


    private static readonly Collider[] _debugOverlapBuffer = new Collider[32];

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = _objectTransform.position;
        Vector3 forward = _objectTransform.forward;

        // ======================================================
        // 1. ESFERA DE DETECCIÓN (radio = _sensorLength)
        // ======================================================
        Gizmos.color = new Color(0f, 1f, 1f, 0.12f);
        Gizmos.DrawWireSphere(origin, _sensorLength);

        // ======================================================
        // 2. COLLIDERS DETECTADOS POR OverlapSphereNonAlloc
        // ======================================================
        if (Application.isPlaying && _obstacleBehaviour != null)
        {
            int count = Physics.OverlapSphereNonAlloc(
                origin, _sensorLength, _debugOverlapBuffer, _obstacleMask);

            for (int i = 0; i < count; i++)
            {
                Collider col = _debugOverlapBuffer[i];
                if (col == null) continue;

                // Ignorar el propio enemigo (misma lógica que el behaviour)
                if (col.transform == _objectTransform || col.transform.IsChildOf(_objectTransform))
                    continue;

                // Punto más cercano del collider al enemigo
                Vector3 closest = col.ClosestPoint(origin);
                Vector3 toObs = closest - origin;
                float distance = toObs.magnitude;

                // Esfera en el punto de contacto
                Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.85f);
                Gizmos.DrawWireSphere(closest, 0.15f);

                if (distance < 0.001f)
                {
                    // Solapado: no hay dirección, marca solo el collider
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
                    continue;
                }

                // Línea origen → punto de contacto (rojo)
                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin, closest);

                // Normal sintética = dirección al obstáculo
                Vector3 normal = toObs / distance;

                // Repulsión (pushAway = -normal) en blanco
                Gizmos.color = Color.white;
                Gizmos.DrawRay(origin, -normal * 1.2f);

                // Deslizamiento tangencial en amarillo
                Vector3 slideDir = Vector3.Cross(Vector3.up, normal).normalized;
                Gizmos.color = new Color(1f, 1f, 0f, 0.6f);
                Gizmos.DrawRay(origin, slideDir * 1.2f);

                // Intensidad visual: cuanto más cerca, más gruesa la línea
                // (Gizmos no soporta grosor, así que dibujamos un segundo rayo desplazado)
                float intensity = 1f - (distance / _sensorLength);
                if (intensity > 0.5f)
                {
                    Gizmos.color = new Color(1f, 0.2f, 0.2f, intensity);
                    Gizmos.DrawLine(origin, closest);
                }
            }
        }

        // ======================================================
        // 3. SENSOR INTERIOR FRONTAL (raycast corto, sin cambios)
        // ======================================================
        if (Physics.Raycast(origin, forward, out RaycastHit interiorHit,
                            _interiorSensorLength, _obstacleMask))
        {
            Gizmos.color = new Color(1f, 0.4f, 0f);
            Gizmos.DrawLine(origin, interiorHit.point);
            Gizmos.DrawWireSphere(interiorHit.point, 0.12f);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(interiorHit.point, interiorHit.normal * 0.6f);
        }
        else
        {
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.5f);
            Gizmos.DrawLine(origin, origin + forward * _interiorSensorLength);
        }

        // ======================================================
        // 4. FIX 1: RAYO HACIA LA DIRECCIÓN REAL DE MOVIMIENTO
        // ======================================================
        if (Application.isPlaying && _chasing != null)
        {
            Vector3 idealMovement = _chasing.LastIdealMovement;
            Vector3 flatMove = new Vector3(idealMovement.x, 0f, idealMovement.z);

            if (flatMove.sqrMagnitude > 0.01f)
            {
                flatMove.Normalize();

                if (Physics.Raycast(origin, flatMove, out RaycastHit moveHit,
                                    _sensorLength, _obstacleMask))
                {
                    Gizmos.color = new Color(0f, 0.9f, 1f);
                    Gizmos.DrawLine(origin, moveHit.point);
                    Gizmos.DrawWireSphere(moveHit.point, 0.15f);

                    Gizmos.color = Color.white;
                    Gizmos.DrawRay(moveHit.point, moveHit.normal * 0.7f);
                }
                else
                {
                    Gizmos.color = new Color(0f, 0.9f, 1f, 0.35f);
                    Gizmos.DrawLine(origin, origin + flatMove * _sensorLength);
                }
            }

            // Referencia visual: intención de movimiento (magenta)
            if (idealMovement.sqrMagnitude > 0.01f)
            {
                Gizmos.color = new Color(1f, 0f, 1f, 0.6f);
                Gizmos.DrawRay(origin, idealMovement.normalized * 3f);
            }
        }

        // ======================================================
        // 5. VECTOR FINAL DE EVASIÓN (lo que se aplica a la velocidad)
        // ======================================================
        if (Application.isPlaying && _obstacleBehaviour != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(origin, _obstacleBehaviour.LastAvoidanceVector);
        }
    }
}