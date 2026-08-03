using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Nucleo del health system. Solo maneja vida, daño y el ciclo de muerte.
/// No sabe nada de sonido, animacion, VFX ni loot: eso lo escuchan
/// componentes separados suscritos a los eventos de esta clase.
/// </summary>
public class Enemy_HealthSystem_Base : MonoBehaviour, Enemy_Interface_Damage
{
    public enum DeathBehaviorType
    {
        Destroy,
        Deactivate
    }
 
    [Header("Health")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
 
    [Header("Death Behavior")]
    [SerializeField] private DeathBehaviorType _deathBehavior = DeathBehaviorType.Destroy;
    [Tooltip("Tiempo antes de destruir/desactivar el gameObject, para dar lugar a los efectos de muerte (VFX, animacion, etc).")]
    [SerializeField] private float _deathDelay = 0f;
 
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDead => _isDead;
    public bool IsInvulnerable => _isInvulnerable;
 
    /// <summary>Vida actual, disparado en cada golpe.</summary>
    public event Action<float> OnDamaged;
 
    /// <summary>Parte del cuerpo golpeada, disparado en cada golpe.</summary>
    public event Action<BodyPartType> OnPartHit;
 
    /// <summary>Disparado una sola vez, apenas la vida llega a 0 (antes del delay de muerte).</summary>
    public event Action OnDeath;
 
    private bool _isDead;
    private bool _isInvulnerable;
 
    void Start()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
    }
 
    /// <summary>
    /// Mientras esta en true, TakeDamage/TakeDamageFromPart no aplican ningun daño.
    /// Pensado para casos como "el boss es invulnerable mientras tenga minions vivos".
    /// </summary>
    public void SetInvulnerable(bool isInvulnerable)
    {
        _isInvulnerable = isInvulnerable;
    }
 
    public void TakeDamage(float amount)
    {
        TakeDamageFromPart(amount, BodyPartType.Chest);
    }
 
    public void TakeDamageFromPart(float amount, BodyPartType part)
    {
        if (_isDead || _isInvulnerable) return;
 
        _currentHealth -= amount;
 
        OnDamaged?.Invoke(_currentHealth);
        OnPartHit?.Invoke(part);
 
        if (_currentHealth <= 0f)
            Die();
    }
 
    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
 
        OnDeath?.Invoke();
        StartCoroutine(FinalizeDeathRoutine());
    }
 
    private IEnumerator FinalizeDeathRoutine()
    {
        if (_deathDelay > 0f)
            yield return new WaitForSeconds(_deathDelay);
 
        if (_deathBehavior == DeathBehaviorType.Destroy)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
