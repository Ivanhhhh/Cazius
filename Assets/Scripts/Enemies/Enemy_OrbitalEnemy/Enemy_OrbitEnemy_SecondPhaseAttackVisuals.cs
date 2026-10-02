using System.Collections;
using System;
using UnityEngine;
using UnityEngine.VFX;


public class Enemy_OrbitEnemy_SecondPhaseAttackVisuals : MonoBehaviour
{
    [Header("VFX inicial")]
    [Tooltip("El VFX Graph que se activa al iniciar el ataque.")]
    [SerializeField] private VisualEffect attackVfx;

    [Header("Objetos que orbitan")]
    [SerializeField] private Transform orbitObjectA;
    [SerializeField] private Transform orbitObjectB;
    [Tooltip("Centro de la órbita. Si está vacío, se usa este Transform.")]
    [SerializeField] private Transform orbitCenter;
    [Tooltip("Velocidad de rotación en grados por segundo.")]
    [SerializeField] private float orbitSpeed = 180f;
    [SerializeField] private float orbitRadius = 2f;
    [Tooltip("Si está activo, los objetos que orbitan se ocultan cuando el ataque se detiene.")]
    [SerializeField] private bool hideOrbitObjectsOnStop = true;

    [Header("Efecto principal (reemplaza al objeto que subía y bajaba)")]
    [Tooltip("GameObject que se activa junto con el VFX y se desactiva cuando termina.")]
    [SerializeField] private GameObject effectObject;
    [Tooltip("VFX principal. Cuando termina, se corta todo.")]
    [SerializeField] private GameObject effectVfxObject;

    [Tooltip("Solo se usa si 'Detect End By Particle Count' está desactivado.")]
    [SerializeField] private float effectDuration = 3f;

    [Header("Tiempos de la secuencia")]
    [Tooltip("Tiempo que el VFX inicial y los objetos giran solos antes de activar el efecto principal.")]
    [SerializeField] private float anticipationTime = 2f;
    [SerializeField] private bool loop = false;
    [SerializeField] private float loopPause = 0.2f;
    [Tooltip("Desactivalo si otro script llama a StartAttack().")]
    [SerializeField] private bool playOnStart = true;

    [Header("Al terminar")]
    [Tooltip("Destruye ESTE GameObject cuando termina el ataque.")]
    [SerializeField] private bool destroyOnFinish = true;
    [Tooltip("Espera antes de destruir, para que se desvanezcan las partículas del VFX inicial.")]
    [SerializeField] private float destroyDelay = 0.5f;

    /// <summary>Se dispara cuando la secuencia termina por sí sola (no con StopAttack manual).</summary>
    public event Action OnAttackFinished;

    // ---------- Estado interno ----------
    private float _currentAngle;
    private bool _isOrbiting;
    private Coroutine _sequenceRoutine;

    // ---------------------------------------------------------
    //  Ciclo de vida
    // ---------------------------------------------------------

    private void Awake()
    {
        Debug.Log($"[Visuals] Awake en '{name}'. effectObject={(effectObject != null)}, effectVfxObject={(effectVfxObject != null)}", this);

        if (orbitCenter == null)
            orbitCenter = transform;

        if (effectObject != null) effectObject.SetActive(false);
        if (effectVfxObject != null) effectVfxObject.SetActive(false);
    }

    private void Start()
    {
        if (playOnStart)
            StartAttack();
    }

    private void Update()
    {
        if (!_isOrbiting) return;
        UpdateOrbit();
    }

    // ---------------------------------------------------------
    //  API pública
    // ---------------------------------------------------------

    public void StartAttack()
    {
        if (_sequenceRoutine != null)
            StopCoroutine(_sequenceRoutine);

        SetOrbitObjectsActive(true);

        if (attackVfx != null)
            attackVfx.Play();

        _isOrbiting = true;
        _sequenceRoutine = StartCoroutine(AttackSequence());
    }

    /// <summary>Corta todo lo que esté en proceso (corrutina, órbita, VFX y objetos).</summary>
    public void StopAttack()
    {
        if (_sequenceRoutine != null)
        {
            StopCoroutine(_sequenceRoutine);
            _sequenceRoutine = null;
        }

        StopEverything();
    }

    // ---------------------------------------------------------
    //  Lógica interna
    // ---------------------------------------------------------

    private IEnumerator AttackSequence()
    {
        do
        {
            // Fase 1: anticipación (giran solos el VFX inicial y los objetos)
            yield return new WaitForSeconds(anticipationTime);

            // Fase 2: efecto principal
            ActivateEffect();
            yield return WaitForEffectEnd();
            DeactivateEffect();

            if (loop)
                yield return new WaitForSeconds(loopPause);

        } while (loop);

        FinishAttack();
    }

    private void ActivateEffect()
    {
        // Primero el objeto, por si el VFX vive adentro y está inactivo.
        if (effectObject != null)
            effectObject.SetActive(true);

        if (effectVfxObject != null)
            effectVfxObject.SetActive(true);
        
        if (effectVfxObject != null)
        {
            var vfx = effectVfxObject.GetComponentInChildren<VisualEffect>(true);
            Debug.Log($"[Visuals] activeInHierarchy={effectVfxObject.activeInHierarchy}, " +
                      $"VisualEffect={(vfx != null ? "OK" : "NO ENCONTRADO")}, " +
                      $"enabled={(vfx != null && vfx.enabled)}, " +
                      $"asset={(vfx != null && vfx.visualEffectAsset != null)}, " +
                      $"initialEvent='{(vfx != null ? vfx.initialEventName : "-")}'");
        }
    }

    private void DeactivateEffect()
    {
        if (effectVfxObject != null)
            effectVfxObject.SetActive(false);

        if (effectObject != null)
            effectObject.SetActive(false);
    }

    private IEnumerator WaitForEffectEnd()
    {
            yield return new WaitForSeconds(effectDuration);
    }

    private void FinishAttack()
    {
        _sequenceRoutine = null;
        StopEverything();

        OnAttackFinished?.Invoke();

        if (destroyOnFinish)
            Destroy(gameObject, destroyDelay);
    }

    private void StopEverything()
    {
        _isOrbiting = false;

        if (attackVfx != null)
            attackVfx.Stop();

        DeactivateEffect();   // ya apaga effectVfxObject y effectObject

        if (hideOrbitObjectsOnStop)
            SetOrbitObjectsActive(false);
    }

    private void SetOrbitObjectsActive(bool active)
    {
        if (orbitObjectA != null) orbitObjectA.gameObject.SetActive(active);
        if (orbitObjectB != null) orbitObjectB.gameObject.SetActive(active);
    }

    private void UpdateOrbit()
    {
        _currentAngle += orbitSpeed * Time.deltaTime;

        if (orbitObjectA != null)
            orbitObjectA.position = GetOrbitPosition(_currentAngle);

        if (orbitObjectB != null)
            orbitObjectB.position = GetOrbitPosition(_currentAngle + 180f);
    }

    private Vector3 GetOrbitPosition(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius;
        return orbitCenter.position + offset;
    }

    // ---------------------------------------------------------
    //  Gizmos
    // ---------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Transform center = orbitCenter != null ? orbitCenter : transform;

        Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.6f);
        Gizmos.DrawWireSphere(center.position, orbitRadius);
    }
}