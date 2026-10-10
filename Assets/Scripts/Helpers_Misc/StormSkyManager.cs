using System.Collections;
using UnityEngine;

public class StormSkyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light _lightningLight;
    [SerializeField] private Material _cloudMaterial;

    [Header("Lightning Timing")]
    [SerializeField] private float _timeBetweenLightning = 8f;
    [SerializeField] private float _randomTimeOffset = 4f;

    [Header("Lightning Light")]
    [SerializeField] private float _maxLightIntensity = 3f;

    [Header("Cloud Flash")]
    [SerializeField] private string _cloudColorProperty = "_CloudColor";
    [SerializeField] private float _maxCloudHDRMultiplier = 4f;

    [Header("Flash Settings")]
    [SerializeField] private int _flashCount = 2;
    [SerializeField] private float _flashInDuration = 0.06f;
    [SerializeField] private float _flashOutDuration = 0.12f;
    [SerializeField] private float _timeBetweenFlashes = 0.08f;

    private Color _baseCloudColor;
    private Coroutine _stormCoroutine;

    private void Start()
    {
        if (_cloudMaterial != null)
        {
            _baseCloudColor = _cloudMaterial.GetColor(_cloudColorProperty);
        }

        if (_lightningLight != null)
        {
            _lightningLight.intensity = 0f;
            _lightningLight.enabled = false;
        }

        _stormCoroutine = StartCoroutine(StormRoutine());
    }

    private IEnumerator StormRoutine()
    {
        while (true)
        {
            float waitTime = _timeBetweenLightning + Random.Range(-_randomTimeOffset, _randomTimeOffset);

            waitTime = Mathf.Max(0.1f, waitTime);

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(LightningRoutine());
        }
    }

    private IEnumerator LightningRoutine()
    {
        if (_lightningLight == null || _cloudMaterial == null)
            yield break;

        _lightningLight.enabled = true;

        for (int i = 0; i < _flashCount; i++)
        {
            yield return StartCoroutine(LerpLightning(0f, _maxLightIntensity, 1f, _maxCloudHDRMultiplier, _flashInDuration));

            yield return StartCoroutine(LerpLightning(_maxLightIntensity, 0f, _maxCloudHDRMultiplier, 1f, _flashOutDuration));

            if (i < _flashCount - 1)
            {
                yield return new WaitForSeconds(_timeBetweenFlashes);
            }
        }

        _lightningLight.intensity = 0f;
        _lightningLight.enabled = false;

        _cloudMaterial.SetColor(_cloudColorProperty, _baseCloudColor);
    }

    private IEnumerator LerpLightning(float startLightIntensity, float endLightIntensity, float startCloudMultiplier, float endCloudMultiplier, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            _lightningLight.intensity = Mathf.Lerp(startLightIntensity, endLightIntensity, t);

            float cloudMultiplier = Mathf.Lerp(startCloudMultiplier, endCloudMultiplier, t);

            Color cloudColor = MultiplyHDRColor(_baseCloudColor, cloudMultiplier);

            _cloudMaterial.SetColor(_cloudColorProperty, cloudColor);

            yield return null;
        }
    }

    private Color MultiplyHDRColor(Color color, float multiplier)
    {
        return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, color.a);
    }
}