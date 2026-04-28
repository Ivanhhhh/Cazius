using System;
using UnityEngine;

public class Enemy_HealthSystem : MonoBehaviour, Enemy_Interface_Damage
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public Action OnDeath;
    public Action<float> OnDamaged;

    void Start()
    {
        _currentHealth = _maxHealth;
        OnDeath += Death;
    }

    public void TakeDamage(float amount)
    {
        TakeDamageFromPart(amount, BodyPartType.Chest);
    }

    public void TakeDamageFromPart(float amount, BodyPartType part)
    {
        _currentHealth -= amount;
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.EnemyGruntSFX, transform.position);
        OnDamaged?.Invoke(_currentHealth);

        // Efecto según la parte
        switch (part)
        {
            case BodyPartType.Head:
                HeadShotEffect();
                break;
            case BodyPartType.Chest:
                ChestShotEffect();
                break;
            case BodyPartType.Legs:
                LegShotEffect();
                break;
            case BodyPartType.Arms:
                ArmShotEffect();
                break;
        }

        if (_currentHealth <= 0)
            OnDeath?.Invoke();
    }

    void HeadShotEffect()
    {
        Debug.Log("Headshot");
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.CriticalHitSFX, transform.position);
    }

    void ArmShotEffect()
    {
        Debug.Log("Arm Shot");
    }

    void ChestShotEffect()
    {
        Debug.Log("Chest shot");
    }

    void LegShotEffect()
    {
        Debug.Log("Leg shot");
    }
    void Death()
    {
        Destroy(gameObject);
    }
}
