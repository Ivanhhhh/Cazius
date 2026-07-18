using System;
using UnityEngine;

public class Enemy_SUPERHEALTHSYSTEM : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    public Action OnDeath;
    public Action<float> OnDamaged;
    private bool _isDead;

    [SerializeField] private Enemy_ShakeDamage enemy_ShakeDamageScript;

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
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.CasterDamgeSFX, transform.position);
        OnDamaged?.Invoke(_currentHealth);
        enemy_ShakeDamageScript.OnDamage();


        // Efecto según la parte
        switch (part)
        {
            case BodyPartType.Head:
                HeadShotEffect();
                break;
            case BodyPartType.Chest:
                ChestShotEffect();
                break;
            case BodyPartType.Left_Leg:
                LeftLegShotEffect();
                break;
            case BodyPartType.Right_Leg:
                RightLegShotEffect();
                break;
            case BodyPartType.Left_Arm:
                LeftArmShotEffect();
                break;
            case BodyPartType.Right_Arm:
                RightArmShotEffect();
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

    void LeftArmShotEffect()
    {
        Debug.Log("Left Arm Shot");
    }
    void RightArmShotEffect()
    {
        Debug.Log("Right Arm Shot");
    }
    void ChestShotEffect()
    {
        Debug.Log("Chest shot");
    }
    void RightLegShotEffect()
    {
        Debug.Log("Right Leg shot");
    }
    void LeftLegShotEffect()
    {
        Debug.Log("Left Leg shot");
    }
    void Death()
    {
        if (_isDead) return;
        _isDead = true;

        OnDeath?.Invoke();
        DieCoroutine();
    }

    void DieCoroutine()
    {
        Destroy(gameObject);
    }
}
