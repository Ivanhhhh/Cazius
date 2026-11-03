
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    [SerializeField] private Transform _shakePivot;

    [Header("Shake Settings")]
    [SerializeField] private float _rotationMultiplier = 12f;
    [SerializeField] private float _pitchAmount = 0.15f;
    [SerializeField] private float _yawAmount = 0.15f;
    [SerializeField] private float _rollAmount = 1f;
    [SerializeField] private float _noiseSpeed = 20f;

    [Header("Continuous Shake")]
    [SerializeField] private float _continuousFadeSpeed = 3f;

    private Quaternion _originalLocalRotation;

    private float _targetContinuousStrength;
    private float _currentContinuousStrength;

    private float _shakeStrength;
    private float _shakeDuration;
    private float _shakeTimer;

    private float _noiseSeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_shakePivot != null)
            _originalLocalRotation = _shakePivot.localRotation;

        _noiseSeed = Random.Range(0f, 1000f);
    }

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }

    private void LateUpdate()
    {
        if (_shakePivot == null)
            return;

        _currentContinuousStrength = Mathf.MoveTowards(
            _currentContinuousStrength,
            _targetContinuousStrength,
            _continuousFadeSpeed * Time.deltaTime
        );

        float impactStrength = 0f;

        if (_shakeTimer < _shakeDuration)
        {
            float progress = _shakeTimer / _shakeDuration;

            impactStrength = _shakeStrength * (1f - progress);

            _shakeTimer += Time.deltaTime;
        }

        float totalStrength = _currentContinuousStrength + impactStrength;

        if (totalStrength <= 0.0001f)
        {
            _shakePivot.localRotation = _originalLocalRotation;
            return;
        }

        float intensity = totalStrength * _rotationMultiplier;
        float time = Time.time * _noiseSpeed;

        float pitch = (Mathf.PerlinNoise(_noiseSeed, time) * 2f - 1f) * intensity * _pitchAmount;
        float yaw = (Mathf.PerlinNoise(_noiseSeed + 100f, time) * 2f - 1f) * intensity * _yawAmount;
        float roll = (Mathf.PerlinNoise(_noiseSeed + 200f, time) * 2f - 1f) * intensity * _rollAmount;

        _shakePivot.localRotation = _originalLocalRotation * Quaternion.Euler(pitch, yaw, roll);
    }

    public void CameraShake(float shakeStrength, float shakeDuration)
    {
        if (_shakePivot == null)
            return;

        _shakeStrength = Mathf.Max(0f, shakeStrength);
        _shakeDuration = Mathf.Max(0f, shakeDuration);
        _shakeTimer = 0f;
    }

    public void SetContinuousShake(float strength)
    {
        _targetContinuousStrength = Mathf.Max(0f, strength);
    }

    private void OnDisable()
    {
        _targetContinuousStrength = 0f;
        _currentContinuousStrength = 0f;

        _shakeStrength = 0f;
        _shakeDuration = 0f;
        _shakeTimer = 0f;

        if (_shakePivot != null)
            _shakePivot.localRotation = _originalLocalRotation;

        if (Instance == this)
            Instance = null;
    }
}
