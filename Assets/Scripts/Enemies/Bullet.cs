using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float _lifeTime = 5f; 

    [Header("Visual Effects")]
    [SerializeField] private GameObject _impactEffectPrefab; 

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }
}
