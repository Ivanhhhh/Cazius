using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using System;

public class Enemy_MeleeEnemy_Data : MonoBehaviour
{
    [Header("Patrolling")]
    [SerializeField] private LayerMask _nodes;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private int _MaximumAmountOfNodes;

    [Header("Chasing")]
    [SerializeField] private float _SprintRadius;
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _attackCooldown;

    [Header("Field of View")]
    [SerializeField] private float _radiusVision = 15f;
    [Range(0, 360)][SerializeField] private float _horizontalAngleVision = 90f;
    [Range(0, 360)][SerializeField] private float _verticalAngleVision = 180f;
    [SerializeField] private LayerMask _lineOfSightLayerMask;
    [SerializeField] private float _aimOffset;

    [Header("First Attack")]
    [SerializeField] private float _firstAttackDuration;
    [SerializeField] private float _firsAttackPreparationTime;
    [SerializeField] private Collider _attackCollider;

    [Header("Second Attack")]
    [SerializeField] private float _secondAttackPreparationTime;
    [SerializeField] private float _spinTime;
    [SerializeField] private float _objectSpeedWhileSpinning;
    [SerializeField] private float _spinSpeed;

    [Header("Stun")]
    [SerializeField] private float _stunDuration;
    public bool _isStunned;

    [Header("General")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _selfObjectTransform;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] public float _timeBeforeReaction;
    [SerializeField] public bool _isAggresiveOnStart;
    [SerializeField] public bool _needTimeForFirstReaction;

    [Header("Health")]
    [SerializeField] public Enemy_HealthSystem _healthSystem;

    [Header("Speed Boost (Distance)")]
    [SerializeField] private float _distanceThreshold = 10f;
    [SerializeField] private float _stopBoostingThreshold;
    [SerializeField] private float _movementSpeedMultiplier = 2f;
    [SerializeField] private Animator _enemyAnimator;
    [SerializeField] private float _animSpeedMultiplier = 2f;

    public Collider AttackCollider => _attackCollider;

    [Header("Behaviours")]
    public Enemy_PatrollingBehaviour _patrolling { get; private set; }
    public Enemy_ChasingBehaviour _chasing { get; private set; }
    public Enemy_FieldOfViewBehaviour _fieldOfView { get; private set; }
    public Enemy_FirstAttackBehaviour _firstAttack { get; private set; }
    public Enemy_SecondAttackBehaviour _secondAttack { get; private set; }
    public Enemy_SpeedBoostBehaviour _speedBoost { get; private set; }

    // Diccionarios para modificadores de velocidad
    private Dictionary<object, float> _speedMultipliers = new Dictionary<object, float>();
    private Dictionary<object, float> _speedAdditives = new Dictionary<object, float>();

    // Evento (por si otros sistemas quieren reaccionar al boost)
    public event Action _onBoostActivated;

    // ✅ Flag local: la fuente de verdad de "hay boost activo"
    public bool IsBoostActive { get; private set; }

    void Awake()
    {
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

    void InitializeBehaviours()
    {
        _patrolling = new Enemy_PatrollingBehaviour(_nodes, _detectionRadius, _MaximumAmountOfNodes, _patrolSpeed, _selfObjectTransform, _agent, _healthSystem);
        _chasing = new Enemy_ChasingBehaviour(_playerTransform, _attackRadius, _agent, _selfObjectTransform, _chaseSpeed, _attackCooldown);
        _fieldOfView = new Enemy_FieldOfViewBehaviour(_playerTransform, _radiusVision, _selfObjectTransform, _horizontalAngleVision, _verticalAngleVision, _lineOfSightLayerMask, _aimOffset);
        _firstAttack = new Enemy_FirstAttackBehaviour(_firsAttackPreparationTime, _firstAttackDuration, _stunDuration, _agent, _attackCollider, this);
        _secondAttack = new Enemy_SecondAttackBehaviour(_spinSpeed, _objectSpeedWhileSpinning, _secondAttackPreparationTime, _spinTime, _playerTransform, _selfObjectTransform, _agent);
        _speedBoost = new Enemy_SpeedBoostBehaviour(
            _selfObjectTransform,
            _playerTransform,
            this,
            _distanceThreshold,
            _stopBoostingThreshold,
            _movementSpeedMultiplier,
            _enemyAnimator,
            _animSpeedMultiplier
        );

        RecalculateSpeed();
    }

    // ====== SISTEMA DE MODIFICADORES DE VELOCIDAD ======

    public void AddSpeedMultiplier(object source, float multiplier)
    {
        _speedMultipliers[source] = multiplier;
        RecalculateSpeed();
    }

    public void RemoveSpeedMultiplier(object source)
    {
        if (_speedMultipliers.Remove(source))
            RecalculateSpeed();
    }

    public void AddSpeedAdditive(object source, float additive)
    {
        // ✅ Barrera definitiva: si hay boost activo y el additivo es negativo, NO ENTRA.
        // No depende de _speedBoost (que puede ser null) ni de timing.
        if (additive < 0f && IsBoostActive)
        {
            Debug.Log($"[Speed] Slow RECHAZADO (src={source.GetType().Name}, add={additive}, boost activo)");
            return;
        }

        _speedAdditives[source] = additive;
        Debug.Log($"[Speed] Additive ACEPTADO (src={source.GetType().Name}, add={additive})");
        RecalculateSpeed();
    }

    public void RemoveSpeedAdditive(object source)
    {
        if (_speedAdditives.Remove(source))
            RecalculateSpeed();
    }

    private void RecalculateSpeed()
    {
        if (_chasing == null) return;

        float finalSpeed = _chaseSpeed;

        // ✅ Usamos el flag local, no _speedBoost.IsCurrentlyBoosted
        bool isBoosted = IsBoostActive;

        foreach (float add in _speedAdditives.Values)
        {
            if (isBoosted && add < 0f) continue;   // red de seguridad
            finalSpeed += add;
        }

        foreach (float mult in _speedMultipliers.Values)
            finalSpeed *= mult;

        finalSpeed = Mathf.Max(0f, finalSpeed);

        _chasing.SetChaseSpeed(finalSpeed);
    }

    public void RecalculateSpeedNow() => RecalculateSpeed();

    public void SetChaseSpeed(float newBaseSpeed)
    {
        _chaseSpeed = newBaseSpeed;
        RecalculateSpeed();
    }

    public float GetBaseChaseSpeed() => _chaseSpeed;

    public float GetCurrentChaseSpeed()
    {
        float speed = _chaseSpeed;
        bool isBoosted = IsBoostActive;

        foreach (float add in _speedAdditives.Values)
        {
            if (isBoosted && add < 0f) continue;
            speed += add;
        }
        foreach (float mult in _speedMultipliers.Values) speed *= mult;
        return Mathf.Max(0f, speed);
    }

    // ====== BOOST ======

    /// <summary>
    /// Llamado por Enemy_SpeedBoostBehaviour al activar/desactivar el boost.
    /// Pone el flag local, limpia slows previos si es activación, y avisa por evento.
    /// </summary>
    public void SetBoostActive(bool active)
    {
        IsBoostActive = active;

        if (active)
        {
            // ✅ Borrar cualquier slow que ya estuviera en el diccionario
            var keysToRemove = new List<object>();
            foreach (var kvp in _speedAdditives)
                if (kvp.Value < 0f) keysToRemove.Add(kvp.Key);

            foreach (var key in keysToRemove)
                _speedAdditives.Remove(key);

            if (keysToRemove.Count > 0)
                Debug.Log($"[Boost] Eliminados {keysToRemove.Count} slows previos");

            _onBoostActivated?.Invoke();   // avisa (AngelDemonAnim cancela su corrutina)
        }

        RecalculateSpeed();
    }

    // ====== AUXILIARES ======

    private Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.y;
        return new Vector3(
            Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)
        );
    }

    public IEnumerator WaitForFirstReaction()
    {
        yield return new WaitForSeconds(_timeBeforeReaction);
        Debug.Log($"Tiempo de espera inicial: {_timeBeforeReaction}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _radiusVision);

        Vector3 angleLeft = transform.rotation * Quaternion.Euler(0, -_horizontalAngleVision / 2, 0) * Vector3.forward;
        Vector3 angleRight = transform.rotation * Quaternion.Euler(0, _horizontalAngleVision / 2, 0) * Vector3.forward;
        Vector3 angleUp = transform.rotation * Quaternion.Euler(-_verticalAngleVision / 2, 0, 0) * Vector3.forward;
        Vector3 angleDown = transform.rotation * Quaternion.Euler(_verticalAngleVision / 2, 0, 0) * Vector3.forward;

        Gizmos.DrawLine(transform.position, transform.position + angleLeft * _radiusVision);
        Gizmos.DrawLine(transform.position, transform.position + angleRight * _radiusVision);

        Gizmos.color = new Color(1, 1, 1, 0.4f);
        Gizmos.DrawLine(transform.position, transform.position + angleUp * _radiusVision);
        Gizmos.DrawLine(transform.position, transform.position + angleDown * _radiusVision);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        Gizmos.color = new Color(1f, 0.6f, 0f);
        Gizmos.DrawWireSphere(transform.position, _distanceThreshold);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _stopBoostingThreshold);
    }
}