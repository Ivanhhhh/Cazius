using UnityEngine;

public enum SoulState
{
    StandBy,
    Atractting,
    Done
}

public class SoulEnergyMovement : MonoBehaviour
{
    [Header("Estado inicial")]
    [SerializeField] private SoulState estadoActual = SoulState.StandBy;

    // ============================================================
    //  FLOTACIÓN - VELOCIDAD POR POSICIÓN
    // ============================================================

    [Header("Flotación")]
    [Tooltip("Distancia total de punta a punta del recorrido vertical.")]
    [SerializeField] private float distanciaTotal = 0.8f;

    [Tooltip("Velocidad en las puntas (puede ser chica, pero > 0 para que nunca se trabe).")]
    [SerializeField] private float velocidadMinima = 0.15f;

    [Tooltip("Velocidad en el punto pico.")]
    [SerializeField] private float velocidadMaximaFlot = 1.8f;

    [Tooltip("En qué fracción de la distancia (desde la punta) está el pico de velocidad. " +
             "0.5 = exactamente en el centro. 0.3 = pico más cerca de la punta. " +
             "0.7 = pico más cerca del otro lado.")]
    [SerializeField, Range(0.1f, 0.9f)] private float puntoVelocidadMaxima = 0.5f;

    [Tooltip("Curva del perfil. 1 = lineal. 2 = acelera más cerca del pico (más 'imán').")]
    [SerializeField, Range(1f, 4f)] private float curvaPerfil = 1.5f;

    [Tooltip("Opcional: si asignás un Transform, la flotación se calcula alrededor suyo. " +
             "Si es null, el centro sigue al orbe y se mueve con cualquier fuerza externa.")]
    [SerializeField] private Transform centroFlotacionExterno;

    // ============================================================
    //  DETECCIÓN Y ATRACCIÓN
    // ============================================================

    [Header("Detección")]
    [SerializeField] private float radioDeteccion = 3f;
    [SerializeField] private float distanciaRecoger = 0.3f;

    [Header("Cooldown")]
    [Tooltip("Segundos mínimos antes de poder empezar a atraer al player.")]
    [SerializeField] private float cooldownAntesDeAtraer = 1.5f;

    [Tooltip("Si está activo, el cooldown arranca desde que el orbe aparece (OnEnable). " +
             "Si está apagado, arranca desde el Start.")]
    [SerializeField] private bool cooldownDesdeSpawn = true;

    [Tooltip("Random extra al cooldown para que no arranquen todos al mismo tiempo.")]
    [SerializeField] private float cooldownJitter = 0.5f;

    [Header("Offset del objetivo")]
    [Tooltip("Desplazamiento vertical del punto al que se dirigen los orbes. " +
             "Positivo = más arriba del player. Negativo = más abajo.")]
    [SerializeField] private float offsetYObjetivo = 1.0f;

    [Header("Atracción - Velocidad")]
    [SerializeField] private float velocidadInicial = 2f;
    [SerializeField] private float atraccionVelocidadMaxima = 12f;
    [SerializeField] private float aceleracion = 8f;
    [SerializeField] private bool acelerarPorDistancia = true;

    [Header("Atracción - Curva (palanca 360°)")]
    [Tooltip("Qué tan lejos se aleja la palanca de la línea recta. 0 = recto.")]
    [SerializeField] private float curvatura = 1.5f;

    [Tooltip("Si está activo, cada orbe elige un ángulo aleatorio 0-360 en cada atracción.")]
    [SerializeField] private bool anguloAleatorio = true;

    [Tooltip("Ángulo fijo si anguloAleatorio = false.")]
    [SerializeField, Range(0f, 360f)] private float anguloFijo = 90f;

    [Header("Deriva (impulso externo)")]
    [Tooltip("Qué tan rápido decae el impulso. 0 = nunca frena. 2 = frena en 1-2 segundos.")]
    [SerializeField] private float desaceleracionDeriva = 1.5f;
    
    [Header("Recompensa")]
    [Tooltip("Cuánta Soul Energy otorga este orbe al ser recogido.")]
    [SerializeField] private int soulEnergyValue = 1;

    // ---------- Referencias ----------
    private Transform _playerPosition;
    private Vector3 baseFlotacion;
    private float offsetFlotacion;

    // ---------- Flotación ----------
    private float distanciaMinima;
    private float distanciaMaxima;
    private int direccionFlotacion = 1;
    private float velocidadFlotacionActual;

    // ---------- Atracción ----------
    private float velocidadActual;
    private Vector3 puntoInicio;
    private Vector3 puntoControl;
    private float tBezier;
    private float distanciaTotalAtraccion;

    // ---------- Deriva ----------
    private Vector3 velocidadDeriva;

    // ---------- Cooldown ----------
    private float timerCooldown;

    private bool _uiRequested;   // solo esta variable privada, sin serializar


    public float RadioDeteccion => radioDeteccion;

    /// Posición del player + offset en Y. Es lo que usan detección, Bézier y recogida.
    private Vector3 PosicionObjetivo =>
        _playerPosition.position + Vector3.up * offsetYObjetivo;

    // ============================================================
    //  CICLO DE VIDA
    // ============================================================

    private void OnEnable()
    {
        estadoActual = SoulState.StandBy;
        _playerPosition = null;
        offsetFlotacion = 0f;
        velocidadDeriva = Vector3.zero;
        baseFlotacion = transform.position;

        if (cooldownDesdeSpawn)
            timerCooldown = cooldownAntesDeAtraer + Random.Range(0f, cooldownJitter);
    }

    private void OnDisable()
    {
        if (_uiRequested)
        {
            _uiRequested = false;
            SoulEnergyManager.Instance?.RequestHideUI(this);
        }
    }

    private void Start()
    {
        AplicarValoresFlotacion();

        if (!cooldownDesdeSpawn)
            timerCooldown = cooldownAntesDeAtraer + Random.Range(0f, cooldownJitter);

        if (GameManager.Instance != null && GameManager.Instance.Player != null)
            _playerPosition = GameManager.Instance.Player.transform;
    }

    private void AplicarValoresFlotacion()
    {
        distanciaMaxima = distanciaTotal * 0.5f;
        distanciaMinima = -distanciaTotal * 0.5f;

        if (centroFlotacionExterno != null)
        {
            baseFlotacion = centroFlotacionExterno.position;
            offsetFlotacion = transform.position.y - baseFlotacion.y;
        }
        else
        {
            // Sin centro externo: el centro es "donde está el orbe menos su offset".
            baseFlotacion = transform.position - Vector3.up * offsetFlotacion;
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        AplicarValoresFlotacion();
    }

    private void Update()
    {
        // Cooldown
        if (timerCooldown > 0f)
            timerCooldown -= Time.deltaTime;

        // Retry del player
        if (_playerPosition == null &&
            GameManager.Instance != null &&
            GameManager.Instance.Player != null)
        {
            _playerPosition = GameManager.Instance.Player.transform;
        }

        switch (estadoActual)
        {
            case SoulState.StandBy: EstadoFlotando(); break;
            case SoulState.Atractting: EstadoAtraccion(); break;
        }
    }

    // ============================================================
    //  ESTADO: FLOTANDO
    // ============================================================

    private void EstadoFlotando()
    {
        // --- 1) Aplicar deriva (impulso) ---
        if (velocidadDeriva.sqrMagnitude > 0.0001f)
        {
            transform.position += velocidadDeriva * Time.deltaTime;

            velocidadDeriva = Vector3.MoveTowards(
                velocidadDeriva,
                Vector3.zero,
                desaceleracionDeriva * Time.deltaTime
            );
        }

        // --- 2) Reconstruir el centro desde la posición actual ---
        if (centroFlotacionExterno != null)
            baseFlotacion = centroFlotacionExterno.position;
        else
            baseFlotacion = transform.position - Vector3.up * offsetFlotacion;

        // --- 3) Velocidad según posición del offset ---
        float distAPuntaInferior = Mathf.Abs(offsetFlotacion - distanciaMinima);
        float distAPuntaSuperior = Mathf.Abs(offsetFlotacion - distanciaMaxima);
        float distAPunta = Mathf.Min(distAPuntaInferior, distAPuntaSuperior);

        float rango = Mathf.Max(0.0001f, distanciaMaxima - distanciaMinima);
        float distAlPico = rango * puntoVelocidadMaxima;
        float t = Mathf.Clamp01(distAPunta / distAlPico);

        float curva = Mathf.Pow(t, curvaPerfil);
        velocidadFlotacionActual = Mathf.Lerp(velocidadMinima, velocidadMaximaFlot, curva);

        // --- 4) Aplicar movimiento al offset ---
        offsetFlotacion += direccionFlotacion * velocidadFlotacionActual * Time.deltaTime;

        if (offsetFlotacion >= distanciaMaxima)
        {
            offsetFlotacion = distanciaMaxima;
            direccionFlotacion = -1;
        }
        else if (offsetFlotacion <= distanciaMinima)
        {
            offsetFlotacion = distanciaMinima;
            direccionFlotacion = 1;
        }

        // --- 5) Posición final ---
        transform.position = baseFlotacion + Vector3.up * offsetFlotacion;

        // --- 6) Detección ---
        if (_playerPosition == null) return;
        if (timerCooldown > 0f) return;

        float dist = Vector3.Distance(transform.position, PosicionObjetivo);
        if (dist <= radioDeteccion)
        {
            if (!_uiRequested)
            {
                _uiRequested = true;
                SoulEnergyManager.Instance?.RequestShowUI(this);
            }

            IniciarAtraccion();
        }
    }

    // ============================================================
    //  ESTADO: ATRAYENDO
    // ============================================================

    private void IniciarAtraccion()
    {
        velocidadActual = velocidadInicial;
        tBezier = 0f;
        puntoInicio = transform.position;
        distanciaTotalAtraccion = Mathf.Max(0.01f,
            Vector3.Distance(puntoInicio, PosicionObjetivo));

        puntoControl = CalcularPuntoControl(puntoInicio, PosicionObjetivo);
        estadoActual = SoulState.Atractting;
    }

    private Vector3 CalcularPuntoControl(Vector3 inicio, Vector3 fin)
    {
        Vector3 medio = (inicio + fin) * 0.5f;
        Vector3 dir = (fin - inicio).normalized;

        Vector3 perpendicular = Vector3.Cross(dir, Vector3.up);
        if (perpendicular.sqrMagnitude < 0.001f)
            perpendicular = Vector3.Cross(dir, Vector3.right);
        perpendicular.Normalize();

        float angulo = anguloAleatorio ? Random.Range(0f, 360f) : anguloFijo;
        Vector3 palanca = Quaternion.AngleAxis(angulo, dir) * perpendicular;

        return medio + palanca * curvatura;
    }

    private void EstadoAtraccion()
    {
        if (_playerPosition == null)
        {
            if (_uiRequested)
            {
                _uiRequested = false;
                SoulEnergyManager.Instance?.RequestHideUI(this);
            }

            baseFlotacion = transform.position;
            estadoActual = SoulState.StandBy;
            timerCooldown = cooldownAntesDeAtraer + Random.Range(0f, cooldownJitter);
            return;
        }

        Vector3 objetivo = PosicionObjetivo;
        float distanciaAlObjetivo = Vector3.Distance(transform.position, objetivo);

        // --- Aceleración ---
        velocidadActual += aceleracion * Time.deltaTime;
        if (acelerarPorDistancia)
        {
            float factor = Mathf.Clamp01(1f - (distanciaAlObjetivo / radioDeteccion));
            velocidadActual += aceleracion * factor * Time.deltaTime;
        }
        velocidadActual = Mathf.Min(velocidadActual, atraccionVelocidadMaxima);

        // --- Avance sobre la curva ---
        tBezier += (velocidadActual * Time.deltaTime) / distanciaTotalAtraccion;
        tBezier = Mathf.Clamp01(tBezier);

        transform.position = BezierCuadratica(
            puntoInicio,
            puntoControl,
            objetivo,
            tBezier
        );

        // --- Recoger ---
        if (distanciaAlObjetivo <= distanciaRecoger || tBezier >= 1f)
        {
            if (distanciaAlObjetivo > distanciaRecoger)
            {
                IniciarAtraccion();
                return;
            }
            Recoger();
        }
    }

    private Vector3 BezierCuadratica(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float u = 1f - t;
        return u * u * a + 2f * u * t * b + t * t * c;
    }

    // ============================================================
    //  ACCIÓN FINAL
    // ============================================================

    private void Recoger()
    {
        estadoActual = SoulState.Done;

        // Sumar al manager global
        if (SoulEnergyManager.Instance != null)
            SoulEnergyManager.Instance.AddSoulEnergy(soulEnergyValue);

        gameObject.SetActive(false);
    }

    // ============================================================
    //  API PÚBLICA (impulsos externos)
    // ============================================================

    public void AplicarImpulso(Vector3 impulso)
    {
        velocidadDeriva = impulso;
    }

    /// Aplica un impulso con dirección aleatoria en 360°.
    /// horizontal = true → solo en el plano XZ. false → esfera completa 3D.
    public void AplicarImpulsoAleatorio(float velocidadMin, float velocidadMax, bool horizontal = true)
    {
        Vector3 direccion;
        if (horizontal)
        {
            float angulo = Random.Range(0f, 360f);
            direccion = Quaternion.Euler(0f, angulo, 0f) * Vector3.forward;
        }
        else
        {
            direccion = Random.onUnitSphere;
        }

        velocidadDeriva = direccion * Random.Range(velocidadMin, velocidadMax);
    }

    // ============================================================
    //  GIZMOS
    // ============================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaRecoger);

        if (Application.isPlaying)
        {
            // Rango de flotación
            Gizmos.color = Color.cyan;
            Vector3 min = baseFlotacion + Vector3.up * distanciaMinima;
            Vector3 max = baseFlotacion + Vector3.up * distanciaMaxima;
            Gizmos.DrawLine(min, max);
            Gizmos.DrawWireSphere(min, 0.05f);
            Gizmos.DrawWireSphere(max, 0.05f);

            // Centro actual
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(baseFlotacion, 0.08f);

            // Picos de velocidad
            float rango = distanciaMaxima - distanciaMinima;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                baseFlotacion + Vector3.up * (distanciaMinima + rango * puntoVelocidadMaxima), 0.06f);
            Gizmos.DrawWireSphere(
                baseFlotacion + Vector3.up * (distanciaMaxima - rango * puntoVelocidadMaxima), 0.06f);

            // Cooldown activo → círculo gris que se achica
            if (timerCooldown > 0f)
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
                float factor = timerCooldown / Mathf.Max(0.01f, cooldownAntesDeAtraer + cooldownJitter);
                Gizmos.DrawWireSphere(transform.position, radioDeteccion * Mathf.Clamp01(factor));
            }

            // Punto objetivo (player + offset)
            if (_playerPosition != null)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.7f);
                Gizmos.DrawWireSphere(PosicionObjetivo, 0.12f);
            }
        }

        // Curva Bézier durante atracción
        if (estadoActual == SoulState.Atractting && _playerPosition != null)
        {
            Vector3 objetivo = PosicionObjetivo;

            Gizmos.color = Color.cyan;
            Vector3 prev = puntoInicio;
            for (int i = 1; i <= 20; i++)
            {
                float t = i / 20f;
                Vector3 p = BezierCuadratica(puntoInicio, puntoControl, objetivo, t);
                Gizmos.DrawLine(prev, p);
                prev = p;
            }

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(puntoControl, 0.1f);
        }
    }
}