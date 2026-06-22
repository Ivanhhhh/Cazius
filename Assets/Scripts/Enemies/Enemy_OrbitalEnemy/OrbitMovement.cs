using UnityEngine;

public class OrbitMovement : MonoBehaviour
{
    [Header("Referencias de Órbita (Autocompletadas)")]
    public Transform _centerPoint; 
    public float _radius = 5f;     
    public float _orbitSpeed = 2f; 
    
    [Tooltip("Inclinación de la órbita en grados. Ej: (90, 0, 0) hace una órbita vertical.")]
    public Vector3 _orbitTilt = Vector3.zero;

    [Header("Atributos de la Bala")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _lifeTime = 5f;

    [Header("Efecto de Aparición")]
    [Tooltip("Velocidad a la que viaja desde donde spawnea hasta encajar en su órbita")]
    [SerializeField] private float _mergeSpeed = 15f;

    private float _currentAngle;
    private OrbitManager _manager;
    private Rigidbody _rb;
    
    private bool _isBullet = false; 
    private bool _isMerging = true; // Controla si está viajando hacia la órbita

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true; 
    }

    public void InitializeOrbit(float startingAngle, OrbitManager manager)
    {
        _currentAngle = startingAngle;
        _manager = manager;
        _isBullet = false;
        _isMerging = true; // Al inicializarse, tiene que viajar a su lugar
        _rb.isKinematic = true;
    }

    void Update()
    {
        if (_isBullet || _centerPoint == null) return; 

        // 1. Calculamos el círculo plano matemático (Plano XZ original)
        _currentAngle += _orbitSpeed * Time.deltaTime;
        Vector3 localOrbitPosition = new Vector3(Mathf.Cos(_currentAngle) * _radius, 0f, Mathf.Sin(_currentAngle) * _radius);
        
        // MAGIA: Inclinamos ese círculo plano usando los ángulos que pusimos en el Inspector
        Vector3 tiltedOrbitPosition = Quaternion.Euler(_orbitTilt) * localOrbitPosition;

        // Lo alineamos con la rotación real del enemigo en el mundo
        Vector3 worldOffset = _centerPoint.rotation * tiltedOrbitPosition;
        Vector3 idealPosition = _centerPoint.position + worldOffset;

        // 2. Lógica de acercamiento suave vs órbita rígida
        if (_isMerging)
        {
            // Aseguramos que la velocidad de alcance SIEMPRE sea mayor a la velocidad a la que "huye" la órbita
            float orbitLinearSpeed = Mathf.Abs(_orbitSpeed * _radius);
            float safeMergeSpeed = Mathf.Max(_mergeSpeed, orbitLinearSpeed + 5f);

            // Se mueve desde donde está hacia el punto ideal asegurando alcanzarlo
            transform.position = Vector3.MoveTowards(transform.position, idealPosition, safeMergeSpeed * Time.deltaTime);

            // MoveTowards devuelve exactamente la misma posición del target al llegar, sin problemas de decimales
            if (transform.position == idealPosition)
            {
                _isMerging = false;
            }
        }
        else
        {
            // Ya encajó: se queda pegado estrictamente a la matemática para no deformar el círculo
            transform.position = idealPosition;
        }

        // 3. Rotar para mirar hacia el centro
        Vector3 directionToCenter = (_centerPoint.position - transform.position).normalized;
        if (directionToCenter != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToCenter);
        }
    }

    // NUEVO: Ahora recibe la POSICIÓN a la que debe ir, no una dirección generada por un tercero.
    public void FireAsBullet(Vector3 targetPosition, float bulletSpeed)
    {
        if (_isBullet) return; 

        _isBullet = true;
        _isMerging = false; // Cancelamos cualquier viaje de encaje que estuviera haciendo
        _rb.isKinematic = false; 
        
        // Bloqueamos la rotación física para que impactos sutiles no giren la bala visualmente
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (_manager != null)
        {
            _manager.RemoveFromOrbit(this);
        }

        // Calculamos la dirección real y exacta desde LA BALA hasta el JUGADOR
        Vector3 exactDirection = (targetPosition - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(exactDirection);
        _rb.linearVelocity = exactDirection * bulletSpeed;

        Destroy(gameObject, _lifeTime);
    }


}
