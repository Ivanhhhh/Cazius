using System.Collections;
using UnityEngine;

public class VolumetricFogWorldChanger : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private Material _volumetricFogMaterial;

    [Header("Shader Property Names")]
    [SerializeField] private string _colorProperty = "_Colour";
    [SerializeField] private string _densityProperty = "_Density";
    [SerializeField] private string _maxDistanceProperty = "_MaxDistance";
    [SerializeField] private string _anisotropyProperty = "_Anisotropy";
    [SerializeField] private string _anisotropyBlendProperty = "_AnisotropyBlend";
    [SerializeField] private string _scatteringMinProperty = "_ScatteringRemapMin";
    [SerializeField] private string _scatteringMaxProperty = "_ScatteringRemapMax";

    [Header("Eden Fog Settings")]
    [SerializeField] private Color _edenColor = Color.white;
    [SerializeField] private float _edenDensity = 0.05f;
    [SerializeField] private float _edenMaxDistance = 20f;
    [SerializeField] private float _edenAnisotropy = 0.5f;
    [SerializeField] private float _edenAnisotropyBlend = 0.3f;
    [SerializeField] private float _edenScatteringMin = 0.2f;
    [SerializeField] private float _edenScatteringMax = 0.5f;

    [Header("Purgatory Fog Settings")]
    [SerializeField] private Color _purgatoryColor = Color.red;
    [SerializeField] private float _purgatoryDensity = 0.15f;
    [SerializeField] private float _purgatoryMaxDistance = 15f;
    [SerializeField] private float _purgatoryAnisotropy = 0.8f;
    [SerializeField] private float _purgatoryAnisotropyBlend = 0.5f;
    [SerializeField] private float _purgatoryScatteringMin = 0.3f;
    [SerializeField] private float _purgatoryScatteringMax = 0.6f;

    [Header("Transition")]
    [SerializeField] private bool _useSmoothTransition = true;
    [SerializeField] private float _transitionDuration = 1f;

    private Coroutine _transitionCoroutine;

    private void Start()
    {
        StartCoroutine(WaitForWorldChangeManager());
    }

    private IEnumerator WaitForWorldChangeManager()
    {
        while (WorldChangeManager.Instance == null)
        {
            yield return null;
        }

        WorldChangeManager.Instance.SwapToEdenEvent += OnSwapToEden;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += OnSwapToPurgatory;
        OnSwapToEden();
    }

    private void OnDestroy()
    {
        if (WorldChangeManager.Instance == null)
            return;

        WorldChangeManager.Instance.SwapToEdenEvent -= OnSwapToEden;
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= OnSwapToPurgatory;
    }

    private void OnSwapToEden()
    {
        ApplyFogSettings(
            _edenColor,
            _edenDensity,
            _edenMaxDistance,
            _edenAnisotropy,
            _edenAnisotropyBlend,
            _edenScatteringMin,
            _edenScatteringMax
        );
    }

    private void OnSwapToPurgatory()
    {
        ApplyFogSettings(
            _purgatoryColor,
            _purgatoryDensity,
            _purgatoryMaxDistance,
            _purgatoryAnisotropy,
            _purgatoryAnisotropyBlend,
            _purgatoryScatteringMin,
            _purgatoryScatteringMax
        );
    }

    private void ApplyFogSettings(
        Color targetColor,
        float targetDensity,
        float targetMaxDistance,
        float targetAnisotropy,
        float targetAnisotropyBlend,
        float targetScatteringMin,
        float targetScatteringMax)
    {
        if (_volumetricFogMaterial == null)
        {
            Debug.LogWarning("Volumetric Fog Material is missing.");
            return;
        }

        if (_useSmoothTransition)
        {
            if (_transitionCoroutine != null)
                StopCoroutine(_transitionCoroutine);

            _transitionCoroutine = StartCoroutine(SmoothFogTransition(
                targetColor,
                targetDensity,
                targetMaxDistance,
                targetAnisotropy,
                targetAnisotropyBlend,
                targetScatteringMin,
                targetScatteringMax
            ));
        }
        else
        {
            SetFogValues(
                targetColor,
                targetDensity,
                targetMaxDistance,
                targetAnisotropy,
                targetAnisotropyBlend,
                targetScatteringMin,
                targetScatteringMax
            );
        }
    }

    private IEnumerator SmoothFogTransition(
        Color targetColor,
        float targetDensity,
        float targetMaxDistance,
        float targetAnisotropy,
        float targetAnisotropyBlend,
        float targetScatteringMin,
        float targetScatteringMax)
    {
        Color startColor = GetColor(_colorProperty);
        float startDensity = GetFloat(_densityProperty);
        float startMaxDistance = GetFloat(_maxDistanceProperty);
        float startAnisotropy = GetFloat(_anisotropyProperty);
        float startAnisotropyBlend = GetFloat(_anisotropyBlendProperty);
        float startScatteringMin = GetFloat(_scatteringMinProperty);
        float startScatteringMax = GetFloat(_scatteringMaxProperty);

        float timer = 0f;

        while (timer < _transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _transitionDuration;

            SetFogValues(
                Color.Lerp(startColor, targetColor, t),
                Mathf.Lerp(startDensity, targetDensity, t),
                Mathf.Lerp(startMaxDistance, targetMaxDistance, t),
                Mathf.Lerp(startAnisotropy, targetAnisotropy, t),
                Mathf.Lerp(startAnisotropyBlend, targetAnisotropyBlend, t),
                Mathf.Lerp(startScatteringMin, targetScatteringMin, t),
                Mathf.Lerp(startScatteringMax, targetScatteringMax, t)
            );

            yield return null;
        }

        SetFogValues(
            targetColor,
            targetDensity,
            targetMaxDistance,
            targetAnisotropy,
            targetAnisotropyBlend,
            targetScatteringMin,
            targetScatteringMax
        );
    }

    private void SetFogValues(
        Color color,
        float density,
        float maxDistance,
        float anisotropy,
        float anisotropyBlend,
        float scatteringMin,
        float scatteringMax)
    {
        SetColor(_colorProperty, color);
        SetFloat(_densityProperty, density);
        SetFloat(_maxDistanceProperty, maxDistance);
        SetFloat(_anisotropyProperty, anisotropy);
        SetFloat(_anisotropyBlendProperty, anisotropyBlend);
        SetFloat(_scatteringMinProperty, scatteringMin);
        SetFloat(_scatteringMaxProperty, scatteringMax);
    }

    private void SetFloat(string propertyName, float value)
    {
        if (_volumetricFogMaterial.HasProperty(propertyName))
        {
            _volumetricFogMaterial.SetFloat(propertyName, value);
        }
        else
        {
            Debug.LogWarning($"Material does not have float property: {propertyName}");
        }
    }

    private float GetFloat(string propertyName)
    {
        if (_volumetricFogMaterial.HasProperty(propertyName))
        {
            return _volumetricFogMaterial.GetFloat(propertyName);
        }

        Debug.LogWarning($"Material does not have float property: {propertyName}");
        return 0f;
    }

    private void SetColor(string propertyName, Color value)
    {
        if (_volumetricFogMaterial.HasProperty(propertyName))
        {
            _volumetricFogMaterial.SetColor(propertyName, value);
        }
        else
        {
            Debug.LogWarning($"Material does not have color property: {propertyName}");
        }
    }

    private Color GetColor(string propertyName)
    {
        if (_volumetricFogMaterial.HasProperty(propertyName))
        {
            return _volumetricFogMaterial.GetColor(propertyName);
        }

        Debug.LogWarning($"Material does not have color property: {propertyName}");
        return Color.white;
    }
}