using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DiageticAmmoUIManager : MonoBehaviour
{

    [SerializeField] MeshRenderer _backgroundMat;
    [SerializeField] TMP_Text[] textComponent;

    [SerializeField] float _fadeDuration = 0.2f;

    [SerializeField] private float _hiddenVertexOffset = 0.2f;
    [SerializeField] private float _visibleVertexOffset = 0f;

    private bool _scanWantsVisible;
    private bool _inventoryWantsVisible;
    private bool _currentlyVisible;

    private Coroutine _fadeCoroutine;

    private bool _subscribed;
    private void OnEnable()
    {
        InventoryInputHandler.OnInventoryVisibilityChanged += OnInventoryVisibilityChanged;

        StartCoroutine(SubscribeWhenReady());
    }

    private IEnumerator SubscribeWhenReady()
    {
        while (WorldScanManager.Instance == null)
        {
            yield return null;
        }

        WorldScanManager.Instance.ScanActive += OnScanActive;
        WorldScanManager.Instance.ScanDeactivate += OnScanDeactivate;

        _scanWantsVisible =
            WorldScanManager.Instance.IsScanActive;

        _subscribed = true;

        RefreshVisibility();
    }

    private void OnDisable()
    {
        InventoryInputHandler.OnInventoryVisibilityChanged -= OnInventoryVisibilityChanged;

        if (!_subscribed)
            return;

        if (WorldScanManager.Instance != null)
        {
            WorldScanManager.Instance.ScanActive -= OnScanActive;

            WorldScanManager.Instance.ScanDeactivate -= OnScanDeactivate;
        }

        _subscribed = false;
    }

    private void OnScanActive()
    {
        _scanWantsVisible = true;

        RefreshVisibility();
    }

    private void OnScanDeactivate()
    {
        _scanWantsVisible = false;

        RefreshVisibility();
    }

    private void OnInventoryVisibilityChanged(bool visible)
    {
        _inventoryWantsVisible = visible;

        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        bool shouldBeVisible =
            _scanWantsVisible ||
            _inventoryWantsVisible;

        if (shouldBeVisible == _currentlyVisible)
            return;

        _currentlyVisible = shouldBeVisible;

        if (shouldBeVisible)
            EnableObject();
        else
            DisableObject();
    }

    private void EnableObject()
    {
        StartCoroutine(FadeShader(_backgroundMat, "_OpacityMultiplier", 0f, 1f, _fadeDuration));
        StartCoroutine(FadeShader(_backgroundMat, "_VertexOffset", 0.2f, 0f, _fadeDuration));
        StartCoroutine(FadeText(0f, 1f, _fadeDuration));
    }

    private void DisableObject()
    {
        StartCoroutine(FadeShader(_backgroundMat, "_OpacityMultiplier", 1f, 0f, _fadeDuration));
        StartCoroutine(FadeShader(_backgroundMat, "_VertexOffset", 0f, 0.2f, _fadeDuration));
        StartCoroutine(FadeText(1f, 0f, _fadeDuration));
    }

    private IEnumerator FadeShader(MeshRenderer mat, string property, float start, float end, float duration)
    {
        float elapsed = 0f;
        mat.material.SetFloat(property, start);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float value = Mathf.Lerp(start, end, elapsed / duration);
            mat.material.SetFloat(property, value);
            yield return null;
        }

        mat.material.SetFloat(property, end);
    }

    private IEnumerator FadeText(float start, float end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {

            elapsed += Time.unscaledDeltaTime;

            float value = Mathf.Lerp(start,end, elapsed / duration);
            foreach (TMP_Text txtComp in textComponent)
            {
                txtComp.alpha = value;
            }

            yield return null;
        }

        foreach (TMP_Text txtComp in textComponent)
        {
            txtComp.alpha = end;
        }

        yield return null;
    }

}
