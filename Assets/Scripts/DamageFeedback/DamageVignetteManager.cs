
using UnityEngine;

public class DamageVignetteManager : MonoBehaviour
{
    public static DamageVignetteManager Instance { get; private set; }

    private enum VignetteState
    {
        Idle,
        Filling,
        Resetting,
        Releasing
    }

    [Header("References")]
    [SerializeField] private Material _vignetteMaterial;

    [Header("Vignette")]
    [Range(0f, 1f)]
    [SerializeField] private float _maxRadius = 0.86f;
    [Range(0f, 1f)]
    [SerializeField] private float _damageRadius = 0.2f;
    [SerializeField] private float _resetDuration = 0.2f;
    [SerializeField] private float _releaseDuration = 0.2f;

    [Header("Camera Shake")]
    [SerializeField] private float _trackingShakeStrength = 0.025f;
    [SerializeField] private float _damageShakeStrength = 0.12f;
    [SerializeField] private float _damageShakeDuration = 0.3f;

    private static readonly int VignetteRadiusID = Shader.PropertyToID("_Vignette_Radius");

    private EyeCameraController _owner;
    private VignetteState _state = VignetteState.Idle;

    private float _currentRadius;
    private float _startRadius;
    private float _targetRadius;
    private float _timer;
    private float _duration;
    private float _nextFillDuration;

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;

        if (_vignetteMaterial == null || !_vignetteMaterial.HasProperty(VignetteRadiusID))
        {
            Debug.LogError("DamageVignetteManager is missing a valid vignette material.", this);
            enabled = false;
            return;
        }

        _owner = null;
        _state = VignetteState.Idle;

        SetRadius(0f);
    }

    private void Update()
    {
        CameraShakeManager.Instance?.SetContinuousShake(_owner != null ? _trackingShakeStrength : 0f);

        if (_state == VignetteState.Idle)
            return;

        _timer += Time.deltaTime;

        float t = Mathf.Clamp01(_timer / _duration);

        SetRadius(Mathf.Lerp(_startRadius, _targetRadius, t));

        if (t < 1f)
            return;

        if (_state == VignetteState.Resetting && _owner != null)
            StartFill(_nextFillDuration);
        else
            _state = VignetteState.Idle;
    }

    public bool TryStart(EyeCameraController requester, float timeUntilDamage)
    {
        if (!isActiveAndEnabled || requester == null)
            return false;

        if (_owner == requester)
            return true;

        if (_owner != null || _state == VignetteState.Releasing)
            return false;

        _owner = requester;

        SetRadius(0f);
        StartFill(timeUntilDamage);

        CameraShakeManager.Instance?.SetContinuousShake(_trackingShakeStrength);

        return true;
    }

    public void NotifyDamage(EyeCameraController requester, float nextDamageInterval)
    {
        if (_owner != requester)
            return;

        CameraShakeManager.Instance?.CameraShake(_damageShakeStrength, _damageShakeDuration);

        float interval = Mathf.Max(0.01f, nextDamageInterval);
        float resetTime = Mathf.Min(Mathf.Max(0f, _resetDuration), interval * 0.5f);

        _nextFillDuration = Mathf.Max(0.01f, interval - resetTime);

        float targetRadius = Mathf.Min(_damageRadius, _maxRadius);

        if (resetTime <= 0f)
        {
            SetRadius(targetRadius);
            StartFill(_nextFillDuration);
            return;
        }

        StartTransition(VignetteState.Resetting, targetRadius, resetTime);
    }

    public void Release(EyeCameraController requester)
    {
        if (_owner != requester)
            return;

        _owner = null;

        CameraShakeManager.Instance?.SetContinuousShake(0f);

        if (_releaseDuration <= 0f)
        {
            SetRadius(0f);
            _state = VignetteState.Idle;
            return;
        }

        StartTransition(VignetteState.Releasing, 0f, _releaseDuration);
    }

    private void StartFill(float duration)
    {
        StartTransition(VignetteState.Filling, _maxRadius, duration);
    }

    private void StartTransition(VignetteState state, float targetRadius, float duration)
    {
        _state = state;
        _startRadius = _currentRadius;
        _targetRadius = targetRadius;
        _timer = 0f;
        _duration = Mathf.Max(0.01f, duration);
    }

    private void SetRadius(float value)
    {
        _currentRadius = Mathf.Clamp(value, 0f, _maxRadius);
        _vignetteMaterial.SetFloat(VignetteRadiusID, _currentRadius);
    }

    private void OnDisable()
    {
        if (Instance != this)
            return;

        CameraShakeManager.Instance?.SetContinuousShake(0f);

        if (_vignetteMaterial != null && _vignetteMaterial.HasProperty(VignetteRadiusID))
            _vignetteMaterial.SetFloat(VignetteRadiusID, 0f);

        _owner = null;
        _currentRadius = 0f;
        _state = VignetteState.Idle;

        Instance = null;
    }
}
