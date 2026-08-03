using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;
public class Player_CounterAttack : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float _detectionRadius;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private Transform _detectionOrigin; // si lo dejás vacío, usa este mismo transform

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private static readonly int HeadbuttTrigger = Animator.StringToHash("Headbutt");

    [Header("Duración de la ejecución")]
    [Tooltip("Tiempo en segundos que dura la animación de Headbutt. Tiene que matchear la duración real del clip.")]
    [SerializeField] private float _executionDuration = 1.2f;
    private float _executionTimer;

    [Header("IK / Rig")]
    [SerializeField] private Rig _playerRig;
    [SerializeField] private float _velocidadTransicionRig = 2f;

    [Header("State")]
    [SerializeField] private bool _isExecuting;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLogs = true;

    [Header("AOE Damage")]
    [SerializeField] private float _aoeDamageAmount;

    private Enemy_MeleeEnemy_Data _currentTarget;

    void OnEnable()
    {
        GameInputManager.Instance.Controls.Player.CounterAttack.started += TryCounterAttack;
    }

    void OnDisable()
    {
        GameInputManager.Instance.Controls.Player.CounterAttack.started -= TryCounterAttack;
    }

    void Awake()
    {
        if (_animator == null)
            Debug.LogError($"[Player_CounterAttack] Falta asignar el Animator en {gameObject.name}");

        if (_enemyLayerMask.value == 0)
            Debug.LogWarning($"[Player_CounterAttack] _enemyLayerMask está vacío en {gameObject.name}, nunca va a detectar nada");
    }

    void Update()
    {
        ActualizarRig();
        ActualizarTimerEjecucion();
    }

    private void ActualizarTimerEjecucion()
    {
        if (!_isExecuting) return;

        _executionTimer -= Time.deltaTime;

        if (_executionTimer <= 0f)
        {
            FinishExecution();
        }
    }

    private void ActualizarRig()
    {
        if (_playerRig == null) return;

        if (_isExecuting)
        {
            // Corte instantáneo apenas arranca la ejecución
            _playerRig.weight = 0f;
        }
    }

    private void TryCounterAttack(InputAction.CallbackContext context)
    {
        Log("Input de CounterAttack detectado.");

        if (_isExecuting)
        {
            Log("Ignorado: ya hay una ejecución en curso (_isExecuting = true).");
            return;
        }

        Vector3 origin = _detectionOrigin != null ? _detectionOrigin.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, _detectionRadius, _enemyLayerMask);

        Log($"OverlapSphere en {origin}, radio {_detectionRadius} -> {hits.Length} collider(s) detectado(s).");

        if (hits.Length == 0)
        {
            Log("No se encontró ningún collider en la layer de enemy dentro del radio.");
            return;
        }

        foreach (var hit in hits)
        {
            Enemy_MeleeEnemy_Data enemyData = hit.GetComponent<Enemy_MeleeEnemy_Data>();

            if (enemyData == null)
            {
                Log($"'{hit.gameObject.name}' está en la layer de enemy pero no tiene Enemy_MeleeEnemy_Data.");
                continue;
            }

            Log($"'{hit.gameObject.name}' encontrado. _isStunned = {enemyData._isStunned}");

            if (enemyData._isStunned)
            {
                Log($"Target encontrado: {hit.gameObject.name}. Ejecutando.");
                StartExecution(enemyData);
                return;
            }
        }

        Log("Había enemigos en rango, pero ninguno estaba stuneado.");
    }

    private void StartExecution(Enemy_MeleeEnemy_Data target)
    {
        Log($"Ejecutando counter attack sobre '{target.gameObject.name}'.");

        _isExecuting = true;
        _currentTarget = target;
        _executionTimer = _executionDuration;

        if (_animator != null)
        {
            _animator.SetTrigger(HeadbuttTrigger);
        }
        else
        {
            Debug.LogError("[Player_CounterAttack] No se puede disparar la animación: _animator es null.");
        }
    }

    private void FinishExecution()
    {
        Log($"Timer de ejecución terminado. Target actual: {(_currentTarget != null ? _currentTarget.gameObject.name : "null")}");

        DamageAllInArea(); // ahora el daño se aplica al terminar la animación, no al presionar el input

        _isExecuting = false;
        _currentTarget = null;
    }

    private void Log(string message)
    {
        if (_showDebugLogs)
        {
            Debug.Log($"[Player_CounterAttack] {message}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = _detectionOrigin != null ? _detectionOrigin.position : transform.position;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(origin, _detectionRadius);
    }

    private void DamageAllInArea()
    {
        Vector3 origin = _detectionOrigin != null ? _detectionOrigin.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, _detectionRadius, _enemyLayerMask);

        Log($"DamageAllInArea: {hits.Length} collider(s) detectado(s) en el área.");

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy_Interface_Damage>(out var damageable))
            {
                Log($"Aplicando {_aoeDamageAmount} de daño a '{hit.gameObject.name}'.");
                damageable.TakeDamage(_aoeDamageAmount);

                // Libera el stun de cualquier enemigo golpeado por el AOE
                if (hit.TryGetComponent<Enemy_MeleeEnemy_Data>(out var enemyData))
                {
                    enemyData._isStunned = false;
                }
            }
        }
    }
}