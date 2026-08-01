using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class Player_CounterAttack : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float _detectionRadius;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private Transform _detectionOrigin; // si lo dejás vacío, usa este mismo transform

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _executionAnimTrigger = "Execute";

    [Header("State")]
    [SerializeField] private bool _isExecuting;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLogs = true;

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
        // chequeos de referencias faltantes apenas arranca, para no descubrirlo recién al tocar el input
        if (_animator == null)
            Debug.LogError($"[Player_CounterAttack] Falta asignar el Animator en {gameObject.name}");

        if (_enemyLayerMask.value == 0)
            Debug.LogWarning($"[Player_CounterAttack] _enemyLayerMask está vacío en {gameObject.name}, nunca va a detectar nada");
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

        bool foundStunnedTarget = false;

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
                foundStunnedTarget = true;
                StartExecution(enemyData);
                break; // ejecutamos al primero que encontramos y cortamos
            }
        }

        if (!foundStunnedTarget)
        {
            Log("Había enemigos en rango, pero ninguno estaba stuneado.");
        }
    }

    private void StartExecution(Enemy_MeleeEnemy_Data target)
    {
        Log($"Ejecutando counter attack sobre '{target.gameObject.name}'.");

        _isExecuting = true;
        _currentTarget = target;

        if (_animator != null)
        {
            _animator.SetTrigger(_executionAnimTrigger);
        }
        else
        {
            Debug.LogError("[Player_CounterAttack] No se puede disparar la animación: _animator es null.");
        }
    }

    // llamado desde Animation Event al final del clip
    public void OnExecutionAnimationEnd()
    {
        Log($"Animation Event recibido: OnExecutionAnimationEnd. Target actual: {(_currentTarget != null ? _currentTarget.gameObject.name : "null")}");

        if (_currentTarget != null)
        {
            _currentTarget._isStunned = false;
        }
        else
        {
            Debug.LogWarning("[Player_CounterAttack] OnExecutionAnimationEnd llamado pero _currentTarget ya era null. ¿Se llamó dos veces el Animation Event?");
        }

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
}
