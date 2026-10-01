using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy_OrbitEnemy_SecondPhaseAttackVisuals : MonoBehaviour
{
    [Header("VFX")]
    [Tooltip("El VFX Graph que se activará al iniciar el ataque.")]
    [SerializeField] private VisualEffect attackVfx;

    [Header("Objetos que orbitan")]
    [Tooltip("Los dos objetos que girarán alrededor del centro.")]
    [SerializeField] private Transform orbitObjectA;
    [SerializeField] private Transform orbitObjectB;

    [Tooltip("Centro de la órbita. Si está vacío, se usa este Transform.")]
    [SerializeField] private Transform orbitCenter;

    [Tooltip("Velocidad de rotación en grados por segundo.")]
    [SerializeField] private float orbitSpeed = 180f;

    [Tooltip("Radio de la órbita.")]
    [SerializeField] private float orbitRadius = 2f;

    [Header("Objeto que sube y baja")]
    [Tooltip("El objeto que se moverá entre la posición inferior y la superior.")]
    [SerializeField] private Transform movingObject;

    [Tooltip("Posición inferior (punto de partida).")]
    [SerializeField] private Transform bottomPoint;

    [Tooltip("Posición superior (punto de llegada).")]
    [SerializeField] private Transform topPoint;

    [Tooltip("Velocidad a la que sube y baja el objeto (unidades/segundo).")]
    [SerializeField] private float travelSpeed = 3f;

    [Tooltip("Tiempo que permanece arriba antes de bajar.")]
    [SerializeField] private float holdTimeAtTop = 1.5f;

    [Header("Tiempos de la secuencia")]
    [Tooltip("Tiempo que el VFX y los objetos giran solos antes de que empiece el movimiento vertical.")]
    [SerializeField] private float anticipationTime = 2f;

    [Tooltip("Si está activado, la secuencia se repite en bucle.")]
    [SerializeField] private bool loop = false;

    // ---------- Estado interno ----------
    private float _currentAngle;
    private bool _isOrbiting;
    private Coroutine _sequenceRoutine;

    // ---------------------------------------------------------
    //  Ciclo de vida
    // ---------------------------------------------------------

    private void Awake()
    {
        // Si no se asigna un centro, usamos el propio Transform como centro.
        if (orbitCenter == null)
            orbitCenter = transform;

        // Colocamos el objeto móvil en la posición inferior al empezar.
        if (movingObject != null && bottomPoint != null)
            movingObject.position = bottomPoint.position;
    }

    private void Start()
    {
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

    /// <summary>Inicia toda la secuencia del ataque.</summary>
    public void StartAttack()
    {
        // Si ya había una secuencia en marcha, la paramos.
        if (_sequenceRoutine != null)
            StopCoroutine(_sequenceRoutine);

        // Activamos el VFX y empezamos a orbitar.
        if (attackVfx != null)
            attackVfx.Play();

        _isOrbiting = true;
        _sequenceRoutine = StartCoroutine(AttackSequence());
    }

    /// <summary>Detiene el ataque, la órbita y el VFX.</summary>
    public void StopAttack()
    {
        if (_sequenceRoutine != null)
        {
            StopCoroutine(_sequenceRoutine);
            _sequenceRoutine = null;
        }

        _isOrbiting = false;

        if (attackVfx != null)
            attackVfx.Stop();
    }

    // ---------------------------------------------------------
    //  Lógica interna
    // ---------------------------------------------------------

    private IEnumerator AttackSequence()
    {
        do
        {
            // ---- Fase 1: anticipación ----
            // El VFX y los dos objetos giran solos durante anticipationTime.
            yield return new WaitForSeconds(anticipationTime);

            // ---- Fase 2: subida ----
            if (movingObject != null && bottomPoint != null && topPoint != null)
            {
                yield return MoveObject(movingObject, bottomPoint.position, topPoint.position, travelSpeed);

                // ---- Fase 3: espera arriba ----
                yield return new WaitForSeconds(holdTimeAtTop);

                // ---- Fase 4: bajada ----
                yield return MoveObject(movingObject, topPoint.position, bottomPoint.position, travelSpeed);
            }

            // Si no está en loop, terminamos aquí.
            if (!loop)
                break;

            // Pequeña pausa antes de repetir el ciclo.
            yield return new WaitForSeconds(0.2f);

        } while (loop);

        // Al terminar (si no hay loop), dejamos de orbitar y paramos el VFX.
        _isOrbiting = false;
        if (attackVfx != null)
            attackVfx.Stop();

        _sequenceRoutine = null;
    }

    /// <summary>Mueve un Transform a velocidad constante de 'from' a 'to'.</summary>
    private IEnumerator MoveObject(Transform target, Vector3 from, Vector3 to, float speed)
    {
        float distance = Vector3.Distance(from, to);
        if (distance <= 0.0001f || speed <= 0.0001f)
        {
            target.position = to;
            yield break;
        }

        float duration = distance / speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        target.position = to;
    }

    /// <summary>Actualiza la posición de los dos objetos que orbitan.</summary>
    private void UpdateOrbit()
    {
        _currentAngle += orbitSpeed * Time.deltaTime;

        if (orbitObjectA != null)
            orbitObjectA.position = GetOrbitPosition(_currentAngle);

        if (orbitObjectB != null)
            orbitObjectB.position = GetOrbitPosition(_currentAngle + 180f);
    }

    /// <summary>Calcula una posición en el círculo de la órbita para un ángulo dado.</summary>
    private Vector3 GetOrbitPosition(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius;
        return orbitCenter.position + offset;
    }

    // ---------------------------------------------------------
    //  Gizmos para ver los puntos en el editor
    // ---------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Transform center = orbitCenter != null ? orbitCenter : transform;

        // Órbita
        Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.6f);
        Gizmos.DrawWireSphere(center.position, orbitRadius);

        // Puntos inferior y superior
        if (bottomPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(bottomPoint.position, 0.15f);
        }

        if (topPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(topPoint.position, 0.15f);
        }

        // Línea entre ambos
        if (bottomPoint != null && topPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(bottomPoint.position, topPoint.position);
        }
    }
}
