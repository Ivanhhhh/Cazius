using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Patterns.Observer.EventManager_Delegates;

public class SoulEnergyUI : MonoBehaviour
{
    // ============================================================
    //  REFERENCIAS
    // ============================================================

    [Header("Referencias")]
    [SerializeField] private TMP_Text _soulEnergyText;
    [Tooltip("Transform que se escala al cambiar el valor. Si es null, usa este GameObject.")]
    [SerializeField] private Transform _scaleTarget;
    [Tooltip("Opcional: fondo (mesh renderer). Si es null, no se hace fade del fondo.")]
    [SerializeField] private MeshRenderer _backgroundMat;

    // ============================================================
    //  FORMATO Y VALOR
    // ============================================================

    [Header("Formato")]
    [SerializeField] private string _prefix = "Soul Energy: ";

    // ============================================================
    //  VISIBILIDAD (fade in/out)
    // ============================================================

    [Header("Fade")]
    [Tooltip("Duración del fade in/out en segundos.")]
    [SerializeField] private float _fadeDuration = 0.2f;

    [Tooltip("Offset de vertex del mesh cuando está oculto.")]
    [SerializeField] private float _hiddenVertexOffset = 0.2f;

    [Tooltip("Offset de vertex del mesh cuando está visible.")]
    [SerializeField] private float _visibleVertexOffset = 0f;

    [Header("Auto-mostrar al cambiar energía")]
    [Tooltip("Si está activo, el panel se muestra automáticamente al cambiar la energía " +
             "y se oculta después de X segundos.")]
    [SerializeField] private bool _autoShowOnChange = true;

    [Tooltip("Segundos que se queda visible tras un cambio (solo si autoShow está activo).")]
    [SerializeField] private float _autoHideDelay = 2f;

    // ============================================================
    //  CONTADOR RODANTE
    // ============================================================

    [Header("Contador rodante")]
    [Tooltip("Velocidad mínima del contador (valores por segundo).")]
    [SerializeField] private float _minTicksPerSecond = 5f;

    [Tooltip("Velocidad máxima del contador (valores por segundo).")]
    [SerializeField] private float _maxTicksPerSecond = 60f;

    [Tooltip("Multiplicador de velocidad según el delta. Más alto = escala más rápido.")]
    [SerializeField] private float _speedFactor = 0.05f;

    // ============================================================
    //  PULSO DE ESCALA
    // ============================================================

    [Header("Escala reactiva")]
    [Tooltip("Multiplicador de escala mientras se está SUMANDO energía.")]
    [SerializeField] private float _scaleUpMultiplier = 1.2f;

    [Tooltip("Multiplicador de escala mientras se está RESTANDO energía.")]
    [SerializeField] private float _scaleDownMultiplier = 0.85f;

    [Tooltip("Qué tan rápido la escala persigue al objetivo. Más alto = más reactivo.")]
    [SerializeField] private float _scaleLerpSpeed = 10f;

    // ============================================================
    //  ESTADO INTERNO
    // ============================================================

    // Contador rodante
    private int _displayedValue;
    private int _targetValue;
    private float _tickAccumulator;

    // Pulso
    private Vector3 _baseScale;

    // Fade / visibilidad
    private bool _currentlyVisible = true;
    private float _currentAlpha = 1f;
    private Coroutine _fadeCoroutine;

    // Fuentes de visibilidad
    private bool _scanWantsVisible;
    private bool _inventoryWantsVisible;
    private bool _autoShowWantsVisible;
    private readonly HashSet<object> _visibilityRequests = new HashSet<object>();

    private bool _subscribed;
    private Coroutine _autoHideCoroutine;

    // ============================================================
    //  CICLO DE VIDA
    // ============================================================

    private void Start()
    {
        if (_scaleTarget == null) _scaleTarget = transform;
        _baseScale = _scaleTarget.localScale;
        if (SoulEnergyManager.Instance != null)
            SoulEnergyManager.Instance.RegisterUI(this);
    }

    private void OnEnable()
    {
        // Evento de energía
        EventManager.SubscribeToEvent(EventsType.Event_SoulEnergyChanged, OnSoulEnergyChanged);

        // Evento de inventario (opcional, igual que en Diagetic)
        InventoryInputHandler.OnInventoryVisibilityChanged += OnInventoryVisibilityChanged;

        // Sincronizar valor inicial
        if (SoulEnergyManager.Instance != null)
        {
            _targetValue = SoulEnergyManager.Instance.CurrentSoulEnergy;
            _displayedValue = _targetValue;
            UpdateText(_displayedValue);
        }

        // Suscripción diferida al WorldScanManager
        StartCoroutine(SubscribeWhenReady());

        // Estado inicial de visibilidad
        RefreshVisibility();
        if (SoulEnergyManager.Instance != null)
            SoulEnergyManager.Instance.RegisterUI(this);
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeToEvent(EventsType.Event_SoulEnergyChanged, OnSoulEnergyChanged);
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

        _scanWantsVisible = WorldScanManager.Instance.IsScanActive;
        _subscribed = true;

        RefreshVisibility();
    }

    private void Update()
    {
        TickCounter();
        UpdateReactiveScale();
    }

    // ============================================================
    //  API PÚBLICA (peticiones externas)
    // ============================================================

    public void RequestVisibility(object source, bool visible)
    {
        if (source == null) return;

        bool changed = visible
            ? _visibilityRequests.Add(source)
            : _visibilityRequests.Remove(source);

        if (changed) RefreshVisibility();
    }

    public void RequestShow(object source) => RequestVisibility(source, true);
    public void RequestHide(object source) => RequestVisibility(source, false);

    // ============================================================
    //  CALLBACKS DE EVENTOS
    // ============================================================

    private void OnSoulEnergyChanged(params object[] parameters)
    {
        if (parameters == null || parameters.Length == 0) return;
        if (!(parameters[0] is int)) return;

        int newValue = (int)parameters[0];
        if (newValue == _targetValue) return;

        _targetValue = newValue;

        if (_autoShowOnChange)
            TriggerAutoShow();
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

    // ============================================================
    //  LÓGICA DE VISIBILIDAD
    // ============================================================

    private void RefreshVisibility()
    {
        bool shouldBeVisible =
            _scanWantsVisible ||
            _inventoryWantsVisible ||
            _autoShowWantsVisible ||
            _visibilityRequests.Count > 0;

        if (shouldBeVisible == _currentlyVisible) return;
        _currentlyVisible = shouldBeVisible;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeTo(shouldBeVisible ? 1f : 0f, _fadeDuration));
    }

    private void TriggerAutoShow()
    {
        _autoShowWantsVisible = true;
        RefreshVisibility();

        if (_autoHideCoroutine != null) StopCoroutine(_autoHideCoroutine);
        _autoHideCoroutine = StartCoroutine(AutoHideAfterDelay());
    }

    private IEnumerator AutoHideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(_autoHideDelay);

        _autoShowWantsVisible = false;
        RefreshVisibility();
        _autoHideCoroutine = null;
    }

    // ============================================================
    //  FADE
    // ============================================================

    private IEnumerator FadeTo(float endAlpha, float duration)
    {
        float startAlpha = _currentAlpha;
        float elapsed = 0f;

        // Aplicar offsets de vertex inmediatamente (no se interpolan)
        if (_backgroundMat != null)
        {
            float vertexOffset = endAlpha > 0.5f ? _visibleVertexOffset : _hiddenVertexOffset;
            _backgroundMat.material.SetFloat("_VertexOffset", vertexOffset);
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            ApplyAlpha(_currentAlpha);
            yield return null;
        }

        _currentAlpha = endAlpha;
        ApplyAlpha(_currentAlpha);
        _fadeCoroutine = null;
    }

    private void ApplyAlpha(float alpha)
    {
        // Texto
        foreach (var txt in GetComponentsInChildren<TMP_Text>())
            txt.alpha = alpha;

        // Fondo
        if (_backgroundMat != null)
            _backgroundMat.material.SetFloat("_OpacityMultiplier", alpha);
    }

    // ============================================================
    //  CONTADOR RODANTE
    // ============================================================

    private void TickCounter()
    {
        if (_displayedValue == _targetValue) return;

        int delta = Mathf.Abs(_targetValue - _displayedValue);

        float speed = Mathf.Lerp(
            _minTicksPerSecond,
            _maxTicksPerSecond,
            Mathf.Clamp01(1f - 1f / (1f + delta * _speedFactor))
        );

        _tickAccumulator += Time.unscaledDeltaTime * speed;

        while (_tickAccumulator >= 1f && _displayedValue != _targetValue)
        {
            _tickAccumulator -= 1f;
            _displayedValue += (_targetValue > _displayedValue) ? 1 : -1;
        }

        UpdateText(_displayedValue);
    }

    private void UpdateText(int value)
    {
        _soulEnergyText.text = _prefix + value;
    }

    // ============================================================
    //  PULSO
    // ============================================================

    private void UpdateReactiveScale()
    {
        // Determinar escala objetivo según el estado del contador
        Vector3 target = _baseScale;

        if (_displayedValue < _targetValue)
            target = _baseScale * _scaleUpMultiplier;
        else if (_displayedValue > _targetValue)
            target = _baseScale * _scaleDownMultiplier;

        // Lerp framerate-independent hacia esa escala
        float t = 1f - Mathf.Exp(-_scaleLerpSpeed * Time.unscaledDeltaTime);
        _scaleTarget.localScale = Vector3.Lerp(_scaleTarget.localScale, target, t);
    }
}