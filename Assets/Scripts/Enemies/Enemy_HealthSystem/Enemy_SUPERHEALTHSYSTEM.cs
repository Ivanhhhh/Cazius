using System;
using System.Collections;
using UnityEngine;

public class Enemy_SUPERHEALTHSYSTEM : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private AngelEyeCasterVisuals _casterVisuals;
    [SerializeField] private GameObject _soulSpawner;
    [SerializeField] private float _delayBeforeDeath;

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
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        _casterVisuals.EyeDie();
        _soulSpawner.SetActive(true);
        yield return new WaitForSeconds(_delayBeforeDeath);
        Destroy(gameObject);

    }
}
