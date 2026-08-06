using UnityEngine;

/// <summary>
/// Efectos visuales al morir: sangre y fade de dithering.
/// Se agrega solo en los enemigos que tengan estos elementos.
/// </summary>
[RequireComponent(typeof(Enemy_HealthSystem_Base))]
public class Enemy_HealthSystem_Death_VFX : MonoBehaviour
{
    [SerializeField] private BloodVFX _bloodVFX;
    [SerializeField] private DitheredTransparency[] _ditheredTransparencies;

    private Enemy_HealthSystem_Base _healthSystem;

    void Awake()
    {
        _healthSystem = GetComponent<Enemy_HealthSystem_Base>();
    }

    void OnEnable()
    {
        _healthSystem.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        _healthSystem.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (_bloodVFX != null)
            _bloodVFX.BloodDead();

        if (_ditheredTransparencies == null) return;

        foreach (DitheredTransparency dither in _ditheredTransparencies)
        {
            if (dither != null)
                dither.FadeAlphaToZero();
        }
    }
}
