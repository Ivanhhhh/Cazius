
using System.Collections;
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

    private Quaternion _originalLocalRotation;
    private Coroutine _shakeCoroutine;

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
    }

    public void CameraShake(float shakeStrength, float shakeDuration)
    {
        if (_shakePivot == null)
            return;

        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }

        _shakePivot.localRotation = _originalLocalRotation;

        _shakeCoroutine = StartCoroutine(CameraShakeCoroutine(shakeStrength, shakeDuration));
    }

    private IEnumerator CameraShakeCoroutine(float shakeStrength, float shakeDuration)
    {
        float elapsedTime = 0f;
        float seed = Random.Range(0f, 1000f);

        while (elapsedTime < shakeDuration)
        {
            float progress = elapsedTime / shakeDuration;
            float intensity = shakeStrength * (1f - progress) * _rotationMultiplier;
            float time = Time.time * _noiseSpeed;

            float pitch = (Mathf.PerlinNoise(seed, time) * 2f - 1f) * intensity * _pitchAmount;
            float yaw = (Mathf.PerlinNoise(seed + 100f, time) * 2f - 1f) * intensity * _yawAmount;
            float roll = (Mathf.PerlinNoise(seed + 200f, time) * 2f - 1f) * intensity * _rollAmount;

            _shakePivot.localRotation = _originalLocalRotation * Quaternion.Euler(pitch, yaw, roll);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _shakePivot.localRotation = _originalLocalRotation;
        _shakeCoroutine = null;
    }

    private void OnDisable()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }

        if (_shakePivot != null)
            _shakePivot.localRotation = _originalLocalRotation;
    }
}
