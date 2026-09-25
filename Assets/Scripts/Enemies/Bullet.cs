using UnityEngine;
using FactoryPool;
public class Bullet : MonoBehaviour, IPoolable
{
    [Header("Projectile Settings")]
    [SerializeField] private float _lifeTime = 5f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _impactEffectPrefab;

    public string PoolId { get; set; }

    private Rigidbody _rb;
    private float _lifeTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // === IPoolable ===
    public void OnGetFromPool()
    {
        gameObject.SetActive(true);
        _lifeTimer = _lifeTime;

        // Resetear físicas
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        // Si tienes TrailRenderer, ParticleSystem, etc., resetealos aquí
        // Ej: GetComponent<TrailRenderer>()?.Clear();
    }

    public void OnReturnToPool()
    {
        if (_impactEffectPrefab != null)
            Instantiate(_impactEffectPrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    // === Lógica de vida ===
    private void Update()
    {
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            ObjectFactory.Instance.Return(this);
        }
    }

    // === Disparo ===
    public void Disparar(Vector3 direction, float speed)
    {
        if (_rb == null) return;

        // Opcional: rotar la bala hacia la dirección (aunque ya la rotamos desde el enemigo)
        if (direction.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(direction);

        // Resetear velocidad por si acaso (doble seguridad)
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        // Aplicar la nueva velocidad
        _rb.linearVelocity = direction.normalized * speed;
    }
}
