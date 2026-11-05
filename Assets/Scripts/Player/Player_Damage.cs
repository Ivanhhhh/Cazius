using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player_Damage : MonoBehaviour
{
    [SerializeField] private Volume _volume;

    [Header("Damage Camera Shake")]
    [SerializeField] private float _damageShakeStrength = 0.3f;
    [SerializeField] private float _damageShakeDuration = 0.4f;

    [Header("Low Health Sound")]
    [SerializeField] private float _soundStrength = 1f;

    [Header("Damage Flash")]
    [SerializeField] private float _flashIntensity = 0.47f;
    [SerializeField] private float _flashHoldTime = 0.47f;
    [SerializeField] private float _flashFadeStep = 0.01f;
    [SerializeField] private float _flashFadeInterval = 0.1f;

    [Header("Intensity Health State")]
    [SerializeField] private float _amazingIntensity = 0f;
    [SerializeField] private float _greatIntensity = 0.5f;
    [SerializeField] private float _regularIntensity = 0.80f;
    [SerializeField] private float _lowIntensity = 1f;
    [SerializeField] private float _criticIntensity = 1.5f;
    [SerializeField] private float _baseTransitionSpeed = 2f;

    private Vignette _vignette;
    private Coroutine _damageCoroutine;
    private Coroutine _baseCoroutine;
    private HealthStates _currentState = HealthStates.Amazing_100;
    private float _baseIntensity;
    private float _currentIntensity;

    private void Start()
    {
        if (_volume == null)
        {
            Debug.LogError("Global Volume empty");
            return;
        }

        if (!_volume.profile.TryGet(out _vignette))
        {
            Debug.LogError("Viggnete empty");
            return;
        }

        _baseIntensity = 0f;
        SetIntensity(0f);
    }

    public void SetHealthState(HealthStates state)
    {
        bool wasHalfOrLess = IsHalfHealthOrLess();

        _currentState = state;

        if (!wasHalfOrLess && IsHalfHealthOrLess() && SFXManager.Instance != null)
            SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.DamageVoiceSFX, _soundStrength);

        if (_vignette == null) return;

        _baseIntensity = GetIntensityForState(state);

        if (_damageCoroutine != null) return;

        if (_baseCoroutine != null)
            StopCoroutine(_baseCoroutine);

        _baseCoroutine = StartCoroutine(TransitionToBase());
    }

    public void TakeDamageEffect()
    {
        if (CameraShakeManager.Instance != null)
            CameraShakeManager.Instance.CameraShake(_damageShakeStrength, _damageShakeDuration);

        if (_vignette == null) return;

        if (_damageCoroutine != null)
            StopCoroutine(_damageCoroutine);

        if (_baseCoroutine != null)
        {
            StopCoroutine(_baseCoroutine);
            _baseCoroutine = null;
        }

        _damageCoroutine = StartCoroutine(DamageEffect());
    }

    private bool IsHalfHealthOrLess()
    {
        return (int)_currentState >= (int)HealthStates.Regular_50;
    }

    private float GetIntensityForState(HealthStates state)
    {
        switch (state)
        {
            case HealthStates.Amazing_100: return _amazingIntensity;
            case HealthStates.Great_80: return _greatIntensity;
            case HealthStates.Regular_50: return _regularIntensity;
            case HealthStates.Low_30: return _lowIntensity;
            case HealthStates.Critic_20: return _criticIntensity;
            default: return 0f;
        }
    }

    private void SetIntensity(float value)
    {
        _currentIntensity = value;
        _vignette.intensity.Override(value);
    }

    private IEnumerator DamageEffect()
    {
        float peak = Mathf.Max(_flashIntensity, _baseIntensity);
        SetIntensity(peak);

        yield return new WaitForSeconds(_flashHoldTime);

        while (_currentIntensity > _baseIntensity)
        {
            float next = _currentIntensity - _flashFadeStep;

            if (next < _baseIntensity)
                next = _baseIntensity;

            SetIntensity(next);

            yield return new WaitForSeconds(_flashFadeInterval);
        }

        SetIntensity(_baseIntensity);
        _damageCoroutine = null;

        if (_baseCoroutine != null)
            StopCoroutine(_baseCoroutine);

        _baseCoroutine = StartCoroutine(TransitionToBase());
    }

    private IEnumerator TransitionToBase()
    {
        while (!Mathf.Approximately(_currentIntensity, _baseIntensity))
        {
            float next = Mathf.MoveTowards(_currentIntensity, _baseIntensity, _baseTransitionSpeed * Time.deltaTime);
            SetIntensity(next);
            yield return null;
        }

        SetIntensity(_baseIntensity);
        _baseCoroutine = null;
    }
}