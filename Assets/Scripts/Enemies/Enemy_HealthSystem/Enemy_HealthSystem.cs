using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class Enemy_HealthSystem : MonoBehaviour, Enemy_Interface_Damage
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    [Header("Soul Energy")]
    [SerializeField] private GameObject _soulEnergyPrefab;

    public Action OnDeath;
    public Action<float> OnDamaged;

    private bool _isDead;

    [SerializeField] private AngelDemonAnim anim;
    [SerializeField] private DitheredTransparency[] _ditheredTransparencies;

    void Start()
    {
        _currentHealth = _maxHealth;
        OnDeath += Death;
        anim = GetComponent<AngelDemonAnim>();
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
        anim.HeadshotAnim();
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.CriticalHitSFX, transform.position);
    }

    void LeftArmShotEffect()
    {
        anim.LeftArmAnim();
        Debug.Log("Left Arm Shot");
    }
    void RightArmShotEffect()
    {
        anim.RightArmAnim();
        Debug.Log("Right Arm Shot");
    }
    void ChestShotEffect()
    {
        anim.ChestAnim();
        Debug.Log("Chest shot");
    }
    void RightLegShotEffect()
    {
        anim.RightLegAnim();
        Debug.Log("Right Leg shot");
    }
    void LeftLegShotEffect()
    {
        anim.LeftLegAnim();
        Debug.Log("Left Leg shot");
    }
    void Death()
    {
        if (_isDead) return;

        _isDead = true;
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        anim.DieAnim();

        foreach (DitheredTransparency dither in _ditheredTransparencies)
        {
            if (dither != null)
                dither.FadeAlphaToZero();
        }

        yield return new WaitForSeconds(1.2f);

        _soulEnergyPrefab.SetActive(true);
        _soulEnergyPrefab.transform.position = transform.position;
        Debug.Log("Soul Energy Drop");

        gameObject.SetActive(false);
    }
}
