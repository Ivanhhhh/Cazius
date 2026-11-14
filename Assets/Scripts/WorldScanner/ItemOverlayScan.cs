
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOverlayScan : MonoBehaviour
{
    [System.Serializable]
    private class RendererEntry
    {
        public Renderer renderer;
        public int materialIndex;
    }

    [SerializeField] private List<RendererEntry> _renderers = new List<RendererEntry>();

    [SerializeField] private float _transitionDuration = 0.5f;

    private Coroutine _opacityCoroutine;
    private float _currentOpacity;

    private readonly List<Material> _materialInstances = new List<Material>();
    private static readonly int OpacityID = Shader.PropertyToID("_Opacity");

    private void Awake()
    {
        foreach (RendererEntry entry in _renderers)
        {
            if (entry.renderer == null) continue;

            Material[] materials = entry.renderer.materials;

            if (entry.materialIndex < 0 || entry.materialIndex >= materials.Length)
                continue;

            Material material = materials[entry.materialIndex];

            _materialInstances.Add(material);
            material.SetFloat(OpacityID, 0f);
        }
    }

    private void OnEnable()
    {

        WorldScanManager.Instance.ScanActive += ActivateScan;
        WorldScanManager.Instance.ScanDeactivate += DeactivateScan;
    }

    private void OnDisable()
    {

        WorldScanManager.Instance.ScanActive -= ActivateScan;
        WorldScanManager.Instance.ScanDeactivate -= DeactivateScan;
    }

    private void ActivateScan()
    {
        StartOpacityTransition(1f);
    }

    private void DeactivateScan()
    {
        StartOpacityTransition(0f);
    }

    private void StartOpacityTransition(float targetOpacity)
    {
        if (_opacityCoroutine != null)
            StopCoroutine(_opacityCoroutine);

        _opacityCoroutine = StartCoroutine(TransitionOpacity(targetOpacity));
    }

    private IEnumerator TransitionOpacity(float targetOpacity)
    {
        float startOpacity = _currentOpacity;
        float elapsed = 0f;

        if (_transitionDuration <= 0f)
        {
            _currentOpacity = targetOpacity;
            SetOpacity(_currentOpacity);
            _opacityCoroutine = null;
            yield break;
        }

        while (elapsed < _transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / _transitionDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            _currentOpacity = Mathf.Lerp(startOpacity, targetOpacity, t);
            SetOpacity(_currentOpacity);

            yield return null;
        }

        _currentOpacity = targetOpacity;
        SetOpacity(_currentOpacity);
        _opacityCoroutine = null;
    }

    private void SetOpacity(float opacity)
    {
        foreach (Material material in _materialInstances)
        {
            if (material != null)
                material.SetFloat(OpacityID, opacity);
        }
    }
}