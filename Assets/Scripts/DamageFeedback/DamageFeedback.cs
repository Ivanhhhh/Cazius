using Patterns.Observer.EventManager_Delegates;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class DamageFeedback : MonoBehaviour
{
    [SerializeField] float TimeStop;
    [SerializeField] float BackAmount;
    [SerializeField] Rigidbody _rb;
    [SerializeField] VisualEffect BloodEffect;
    [SerializeField] Animator _animator;
    private GameInputManager _gameInputManager;

    private Animator _Animator;
    private PlayerMovement _PlayerMovement;

    [SerializeField] Vector3 BloodPosition;
    private bool _knockbackInProgress;

    [Header("Física del knockback")]
    [SerializeField] float knockbackDrag = 0f;
    [SerializeField] bool ignoreMass = true;

    [Header("Debug")]
    [SerializeField] bool debugKnockback = true;
    [SerializeField] bool debugEveryStep = false;   // muy verboso, solo si lo necesitas

    void Start()
    {
        _rb = GetComponent<Rigidbody>(); 
        _PlayerMovement = GetComponent<PlayerMovement>();
        _Animator = GetComponent<Animator>();
        _gameInputManager = GameInputManager.Instance;
    }

    void OnEnable()
    {
        EventManager.SubscribeToEvent(EventsType.Event_PausePlayer, StopMovingMethod);
        BloodEffect.transform.position = transform.up * 9;
    }

    void OnDisable()
    {
        EventManager.UnsubscribeToEvent(EventsType.Event_PausePlayer, StopMovingMethod);
    }

    private IEnumerator MovingLerp(float time, float backAmount)
    {
        _PlayerMovement.enabled = false;
        _gameInputManager.DisablePlayerMovement();

        // ---- Estado inicial ----
        Vector3 startPos = _rb.position;
        Vector3 startForward = transform.forward;

        Vector3 dir = -transform.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            FinishKnockback();
            yield break;
        }
        dir.Normalize();

        float initialSpeed = backAmount / Mathf.Max(0.01f, time);

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        ForceMode mode = ignoreMass ? ForceMode.VelocityChange : ForceMode.Impulse;
        _rb.AddForce(dir * initialSpeed, mode);

        if (debugKnockback)
        {
            Debug.Log($"[Knockback] INICIO\n" +
                      $"  Start pos   : {startPos}\n" +
                      $"  Forward     : {startForward}\n" +
                      $"  Dir empuje  : {dir}\n" +
                      $"  BackAmount  : {backAmount} (esperado)\n" +
                      $"  TimeStop    : {time} s\n" +
                      $"  Velocidad   : {initialSpeed} u/s  ({mode})");
        }

        // ---- Bucle ----
        float elapsed = 0f;
        float realTime = 0f;
        Vector3 lastPos = _rb.position;
        float totalRealDistance = 0f;
        int steps = 0;

        while (elapsed < time)
        {
            elapsed += Time.fixedDeltaTime;

            if (knockbackDrag > 0f)
            {
                Vector3 v = _rb.linearVelocity;
                Vector3 horizontal = new Vector3(v.x, 0f, v.z);
                horizontal = Vector3.MoveTowards(horizontal, Vector3.zero, knockbackDrag * Time.fixedDeltaTime);
                _rb.linearVelocity = new Vector3(horizontal.x, v.y, horizontal.z);
            }

            yield return new WaitForFixedUpdate();

            // ---- Medición por step ----
            Vector3 currentPos = _rb.position;
            float stepDistance = Vector3.Distance(lastPos, currentPos);
            totalRealDistance += stepDistance;
            realTime += Time.fixedDeltaTime;
            steps++;

            if (debugKnockback && debugEveryStep)
            {
                Debug.Log($"[Knockback] Step {steps:D3} | t={realTime:F3}s | " +
                          $"Δstep={stepDistance:F4} | vel={_rb.linearVelocity.magnitude:F3} | pos={currentPos}");
            }

            lastPos = currentPos;
        }

        // ---- Estado final ----
        Vector3 endPos = _rb.position;
        Vector3 desplazamiento = endPos - startPos;
        float distanciaReal = desplazamiento.magnitude;
        float distanciaEsperada = backAmount;
        float error = distanciaReal - distanciaEsperada;
        float porcentaje = distanciaEsperada > 0.0001f ? (distanciaReal / distanciaEsperada) * 100f : 0f;

        if (debugKnockback)
        {
            Debug.Log($"[Knockback] FIN\n" +
                      $"  End pos            : {endPos}\n" +
                      $"  Desplazamiento     : {desplazamiento}\n" +
                      $"  Distancia esperada : {distanciaEsperada:F3}\n" +
                      $"  Distancia real     : {distanciaReal:F3}\n" +
                      $"  Distancia acumulada: {totalRealDistance:F3} (suma de steps)\n" +
                      $"  Error              : {error:F3} ({(error >= 0 ? "+" : "")}{porcentaje:F1}%)\n" +
                      $"  Steps totales      : {steps}\n" +
                      $"  Tiempo real        : {realTime:F3} s (objetivo {time} s)");
        }

        FinishKnockback();
    }

    private void FinishKnockback()
    {
        if (_rb != null && !_rb.isKinematic)
        {
            Vector3 v = _rb.linearVelocity;
            _rb.linearVelocity = new Vector3(0f, v.y, 0f);
        }

        _gameInputManager.EnablePlayerMovement();
        _PlayerMovement.enabled = true;
        _knockbackInProgress = false;
    }

    public void StopMovingMethod(params object[] parameters)
    {
        if (_knockbackInProgress) return;
        _knockbackInProgress = true;

        _animator.SetTrigger("TakeDamage");
        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.HurtedSFX);

        StartCoroutine(MovingLerp(TimeStop, BackAmount));

        BloodEffect.transform.position = transform.position;
        BloodEffect.Play();
    }

    public void ParryAttack(params object[] parameters)
    {
        if (_knockbackInProgress) return;
        _knockbackInProgress = true;
        StartCoroutine(MovingLerp(TimeStop, BackAmount));
    }
}