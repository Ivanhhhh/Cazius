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

    [Header("Boss Phase Colors")]
    [ColorUsage(true, true)]
    [SerializeField] private Color _swapPhaseColor = Color.red;

    [ColorUsage(true, true)]
    [SerializeField] private Color _dieColor = Color.white;

    private Color _baseCloudColor;
    private bool _phaseSwapped;
    private bool _bossDied;

    private void Awake()
    {
        if (_cloudMaterial != null)
            _baseCloudColor = _cloudMaterial.GetColor(_cloudColorProperty);
    }

    private void OnEnable()
    {
        if (BossFightManager.Instance != null)
        {
            BossFightManager.Instance.SwapPhase += OnSwapPhase;
            BossFightManager.Instance.Die += OnBossDie;
        }

        ResetLightning();

        if (_cloudMaterial != null)
        {
            Color color = _bossDied ? _dieColor : _phaseSwapped ? _swapPhaseColor : _baseCloudColor;
            _cloudMaterial.SetColor(_cloudColorProperty, color);
        }

        if (!_phaseSwapped && !_bossDied)
            StartCoroutine(StormRoutine());
    }

    private void OnSwapPhase()
    {
        if (_phaseSwapped || _bossDied) return;

        _phaseSwapped = true;
        StopAllCoroutines();
        ResetLightning();

        if (_cloudMaterial == null) return;

        _cloudMaterial.SetColor(_cloudColorProperty, _baseCloudColor);
        StartCoroutine(LerpCloudColor(_baseCloudColor, _swapPhaseColor, BossFightManager.Instance.TransitionDuration));
    }

    private void OnBossDie()
    {
        if (_bossDied) return;

        _bossDied = true;
        StopAllCoroutines();
        ResetLightning();

        if (_cloudMaterial == null) return;

        Color currentColor = _cloudMaterial.GetColor(_cloudColorProperty);
        StartCoroutine(LerpCloudColor(currentColor, _dieColor, BossFightManager.Instance.TransitionDuration));
    }

    private IEnumerator LerpCloudColor(Color startColor, Color endColor, float duration)
    {
        if (duration <= 0f)
        {
            _cloudMaterial.SetColor(_cloudColorProperty, endColor);
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            _cloudMaterial.SetColor(_cloudColorProperty, Color.Lerp(startColor, endColor, t));
            yield return null;
        }

        _cloudMaterial.SetColor(_cloudColorProperty, endColor);
    }

    private IEnumerator StormRoutine()
    {
        while (!_phaseSwapped && !_bossDied)
        {
            float waitTime = _timeBetweenLightning + Random.Range(-_randomTimeOffset, _randomTimeOffset);
            yield return new WaitForSeconds(Mathf.Max(0.1f, waitTime));

            if (_phaseSwapped || _bossDied) yield break;

            yield return LightningRoutine();
        }
    }

    private IEnumerator LightningRoutine()
    {
        if (_lightningLight == null || _cloudMaterial == null) yield break;

        _lightningLight.enabled = true;

        for (int i = 0; i < _flashCount; i++)
        {
            yield return LerpLightning(0f, _maxLightIntensity, 1f, _maxCloudHDRMultiplier, _flashInDuration);
            yield return LerpLightning(_maxLightIntensity, 0f, _maxCloudHDRMultiplier, 1f, _flashOutDuration);

            if (i < _flashCount - 1)
                yield return new WaitForSeconds(_timeBetweenFlashes);
        }

        ResetLightning();
        _cloudMaterial.SetColor(_cloudColorProperty, _baseCloudColor);
    }

    private IEnumerator LerpLightning(float startIntensity, float endIntensity, float startMultiplier, float endMultiplier, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(timer / duration);

            _lightningLight.intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            float multiplier = Mathf.Lerp(startMultiplier, endMultiplier, t);
            _cloudMaterial.SetColor(_cloudColorProperty, MultiplyHDRColor(_baseCloudColor, multiplier));

            yield return null;
        }
    }

    private Color MultiplyHDRColor(Color color, float multiplier)
    {
        return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, color.a);
    }

    private void ResetLightning()
    {
        if (_lightningLight == null) return;

        _lightningLight.intensity = 0f;
        _lightningLight.enabled = false;
    }

    private void ResetStorm()
    {
        ResetLightning();

        if (_cloudMaterial != null)
            _cloudMaterial.SetColor(_cloudColorProperty, _baseCloudColor);
    }

    private void OnDisable()
    {
        if (BossFightManager.Instance != null)
        {
            BossFightManager.Instance.SwapPhase -= OnSwapPhase;
            BossFightManager.Instance.Die -= OnBossDie;
        }

        StopAllCoroutines();
        ResetStorm();
    }

    private void OnDestroy()
    {
        ResetStorm();
    }
}