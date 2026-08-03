using UnityEngine;

/// <summary>
/// Maneja los SFX de daño y muerte de un enemigo.
/// Cada objeto configura sus propias categorias, sin tocar el health system.
/// </summary>
[RequireComponent(typeof(Enemy_HealthSystem_Base))]
public class Enemy_HealthSystem_Hit_VFX : MonoBehaviour
{
    [Header("Damage SFX")]
    [SerializeField] private SFXManager.SFXCategoryType _onDamageSFX = SFXManager.SFXCategoryType.EnemyGruntSFX;

    [Header("Critical Hit (headshot)")]
    [SerializeField] private bool _playCriticalOnHeadshot = true;
    [SerializeField] private SFXManager.SFXCategoryType _criticalSFX = SFXManager.SFXCategoryType.CriticalHitSFX;

    [Header("Death SFX")]
    [SerializeField] private bool _playDeathSFX;
    [SerializeField] private SFXManager.SFXCategoryType _onDeathSFX;

    private Enemy_HealthSystem_Base _healthSystem;

    void Awake()
    {
        _healthSystem = GetComponent<Enemy_HealthSystem_Base>();
    }

    void OnEnable()
    {
        _healthSystem.OnPartHit += HandlePartHit;
        _healthSystem.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        _healthSystem.OnPartHit -= HandlePartHit;
        _healthSystem.OnDeath -= HandleDeath;
    }

    private void HandlePartHit(BodyPartType part)
    {
        if (SFXManager.Instance == null) return;

        SFXManager.Instance.PlaySFXAtPosition(_onDamageSFX, transform.position);

        if (part == BodyPartType.Head && _playCriticalOnHeadshot)
            SFXManager.Instance.PlaySFXAtPosition(_criticalSFX, transform.position);
    }

    private void HandleDeath()
    {
        if (!_playDeathSFX || SFXManager.Instance == null) return;

        SFXManager.Instance.PlaySFXAtPosition(_onDeathSFX, transform.position);
    }
}
