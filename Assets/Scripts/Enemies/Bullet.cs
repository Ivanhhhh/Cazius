using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _lifeTime = 5f; // Tiempo máximo vivo para no consumir memoria

    [Header("Visual Effects")]
    [SerializeField] private GameObject _impactEffectPrefab; // Opcional: Partículas al chocar

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // 1. Seguro de vida: Si la bala viaja hacia el infinito y no choca con nada, 
        // se destruye sola después de X segundos para evitar lag.
        Destroy(gameObject, _lifeTime);
    }

    // 2. Detección de colisión física

    
}
