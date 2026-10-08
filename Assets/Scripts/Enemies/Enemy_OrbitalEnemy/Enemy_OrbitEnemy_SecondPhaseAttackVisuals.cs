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
    [Tooltip("Destruye ESTE GameObject cuando el VFX inicial se queda sin partículas (después del Stop).")]
    [SerializeField] private bool destroyOnFinish = true;
    [Tooltip("aliveParticleCount llega con unos frames de retraso. Se espera este tiempo antes de confiar en un 0.")]
    [SerializeField] private float particleCountSettleTime = 0.1f;
    [Tooltip("Seguridad: si las partículas nunca llegan a 0, se destruye igual pasado este tiempo.")]
    [SerializeField] private float maxDestroyWait = 5f;

    [Header("Point Light (carga/descarga)")]
    [Tooltip("Light que sube de intensidad durante la anticipación y baja durante el efecto.")]
    [SerializeField] private Light chargeLight;

    [Tooltip("Intensidad en reposo (al inicio y al final).")]
    [SerializeField] private float lightMinIntensity = 0f;

    [Tooltip("Intensidad pico, justo en el momento en que se activa el efecto.")]
    [SerializeField] private float lightPeakIntensity = 8f;

    [Tooltip("Curva de subida. 1 = lineal. >1 = se queda baja y explota al final (más 'carga').")]
    [SerializeField, Range(0.1f, 4f)] private float lightRampUpCurve = 2f;

    [Tooltip("Curva de bajada. 1 = lineal. >1 = baja lento y se apaga al final.")]
    [SerializeField, Range(0.1f, 4f)] private float lightRampDownCurve = 1f;




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
        Debug.Log($"[Visuals] Start. playOnStart={playOnStart}", this);

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
        Debug.Log("[Visuals] StartAttack", this);

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
            // Fase 1: anticipación — la luz sube de min a peak
            //           (pega el pico justo cuando termina la anticipación)
            yield return RampLightAndWait(
                lightMinIntensity,
                lightPeakIntensity,
                anticipationTime,
                lightRampUpCurve
            );

            // En este instante la luz está en peak y activamos el efecto
            ActivateEffect();

            // Fase 2: efecto principal — la luz baja de peak a min a lo largo de effectDuration
            yield return RampLightAndWait(
                lightPeakIntensity,
                lightMinIntensity,
                effectDuration,
                lightRampDownCurve
            );

            DeactivateEffect();

            if (loop)
                yield return new WaitForSeconds(loopPause);

        } while (loop);

        FinishAttack();
    }

    private void ActivateEffect()
    {
        Debug.Log($"[Visuals] ActivateEffect. effectObject={(effectObject != null)}, effectVfxObject={(effectVfxObject != null)}", this);

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



    private void FinishAttack()
    {
        _sequenceRoutine = null;
        StopEverything();   // acá se hace attackVfx.Stop()

        OnAttackFinished?.Invoke();

        if (destroyOnFinish)
            StartCoroutine(DestroyWhenVfxDone());
    }

    private void StopEverything()
    {
        _isOrbiting = false;

        if (attackVfx != null)
            attackVfx.Stop();

        DeactivateEffect();

        if (hideOrbitObjectsOnStop)
            SetOrbitObjectsActive(false);

        if (chargeLight != null)
            chargeLight.intensity = lightMinIntensity;   // <-- nuevo
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

    private IEnumerator RampLightAndWait(float from, float to, float duration, float curve)
    {
        if (chargeLight == null)
        {
            yield return new WaitForSeconds(duration);
            yield break;
        }

        duration = Mathf.Max(0.0001f, duration);
        chargeLight.intensity = from;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            k = Mathf.Pow(k, curve);                 // misma idea que curvaPerfil
            chargeLight.intensity = Mathf.Lerp(from, to, k);
            yield return null;
        }

        chargeLight.intensity = to;                  // asegura el valor exacto al cerrar la fase
    }

    // ---------------------------------------------------------
    //  Gizmos
    // ---------------------------------------------------------

    private IEnumerator DestroyWhenVfxDone()
    {
        if (attackVfx != null)
        {
            // El contador se actualiza con unos frames de retraso: esperamos antes de leerlo.
            yield return new WaitForSeconds(particleCountSettleTime);

            float elapsed = 0f;
            while (attackVfx.aliveParticleCount > 0 && elapsed < maxDestroyWait)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (elapsed >= maxDestroyWait)
            {
                Debug.LogWarning("[Visuals] El VFX inicial sigue con partículas vivas. Se destruye igual.", this);
            }
        }

        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Transform center = orbitCenter != null ? orbitCenter : transform;

        Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.6f);
        Gizmos.DrawWireSphere(center.position, orbitRadius);
    }
}