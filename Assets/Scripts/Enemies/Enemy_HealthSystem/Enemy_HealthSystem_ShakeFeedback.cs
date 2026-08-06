using UnityEngine;

/// <summary>
/// Adaptador que conecta el shake de camara/objeto (Enemy_ShakeDamage) con
/// el health system, sin modificar Enemy_ShakeDamage (viene del asset SmoothShakeFree).
/// </summary>
[RequireComponent(typeof(Enemy_HealthSystem_Base))]
public class Enemy_HealthSystem_ShakeFeedback : MonoBehaviour
{
    [SerializeField] private Enemy_ShakeDamage _shakeOnDamage;

    private Enemy_HealthSystem_Base _healthSystem;

    void Awake()
    {
        _healthSystem = GetComponent<Enemy_HealthSystem_Base>();
        if (_shakeOnDamage == null) _shakeOnDamage = GetComponent<Enemy_ShakeDamage>();
    }

    void OnEnable()
    {
        _healthSystem.OnDamaged += HandleDamaged;
    }

    void OnDisable()
    {
        _healthSystem.OnDamaged -= HandleDamaged;
    }

    private void HandleDamaged(float currentHealth)
    {
        if (_shakeOnDamage != null)
            _shakeOnDamage.OnDamage();
    }
}
