using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DiageticAmmoUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer _backgroundMat;
    [SerializeField] private TMP_Text[] textComponent;

    [Header("Fade")]
    [SerializeField] private float _fadeDuration = 0.2f;
    [SerializeField] private float _hiddenVertexOffset = 0.2f;
    [SerializeField] private float _visibleVertexOffset = 0f;

    // Fuentes de visibilidad internas
    private bool _scanWantsVisible;
    private bool _inventoryWantsVisible;

    // Peticiones externas (aim, reload, etc.)
    private readonly HashSet<object> _visibilityRequests = new HashSet<object>();

    // Estado actual
    private bool _currentlyVisible;

    private bool _subscribed;

    // ====== LIFECYCLE ======

    private void OnEnable()
    {
        InventoryInputHandler.OnInventoryVisibilityChanged += OnInventoryVisibilityChanged;

        StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        InventoryInputHandler.OnInventoryVisibilityChanged -= OnInventoryVisibilityChanged;

        if (_subscribed && WorldScanManager.Instance != null)
        {
            WorldScanManager.Instance.ScanActive -= OnScanActive;
            WorldScanManager.Instance.ScanDeactivate -= OnScanDeactivate;
        }

        _subscribed = false;
    }

    private IEnumerator SubscribeWhenReady()
    {
        while (WorldScanManager.Instance == null)
            yield return null;

        WorldScanManager.Instance.ScanActive += OnScanActive;
        WorldScanManager.Instance.ScanDeactivate += OnScanDeactivate;

        // Sincronizar estado inicial
        _scanWantsVisible = WorldScanManager.Instance.IsScanActive;

        _subscribed = true;

        RefreshVisibility();
    }

    // ====== API PÚBLICA (peticiones externas) ======

    public void RequestVisibility(object source, bool visible)
    {
        if (source == null) return;

        bool changed = visible
            ? _visibilityRequests.Add(source)
            : _visibilityRequests.Remove(source);

        if (changed)
            RefreshVisibility();
    }

    public void RequestShow(object source) => RequestVisibility(source, true);
    public void RequestHide(object source) => RequestVisibility(source, false);

    // ====== EVENTOS INTERNOS ======

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

    // ====== LÓGICA DE VISIBILIDAD ======

    private void RefreshVisibility()
    {
        bool shouldBeVisible =
            _scanWantsVisible ||
            _inventoryWantsVisible ||
            _visibilityRequests.Count > 0;

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
        StartCoroutine(FadeShader(_backgroundMat, "_VertexOffset", _hiddenVertexOffset, _visibleVertexOffset, _fadeDuration));
        StartCoroutine(FadeText(0f, 1f, _fadeDuration));
    }

    private void DisableObject()
    {
        StartCoroutine(FadeShader(_backgroundMat, "_OpacityMultiplier", 1f, 0f, _fadeDuration));
        StartCoroutine(FadeShader(_backgroundMat, "_VertexOffset", _visibleVertexOffset, _hiddenVertexOffset, _fadeDuration));
        StartCoroutine(FadeText(1f, 0f, _fadeDuration));
    }

    // ====== FADES ======

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
            float value = Mathf.Lerp(start, end, elapsed / duration);

            foreach (TMP_Text txtComp in textComponent)
                txtComp.alpha = value;

            yield return null;
        }

        foreach (TMP_Text txtComp in textComponent)
            txtComp.alpha = end;
    }
}
