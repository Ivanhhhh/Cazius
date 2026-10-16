using System;
using System.Collections;
using UnityEngine;

public class UIFadeInFadeOut : MonoBehaviour
{
    public static UIFadeInFadeOut Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup _fadeCanvasGroup;

    [Header("Global Fade Durations")]
    [SerializeField] private float _defaultFadeOutDuration = 0.5f;
    [SerializeField] private float _defaultHoldDuration = 0.2f;
    [SerializeField] private float _defaultFadeInDuration = 0.5f;

    private bool _isFading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_fadeCanvasGroup != null)
        {
            _fadeCanvasGroup.alpha = 0f;
            _fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    public void FadeOutIn(Action onBlack, float? fadeOut = null, float? hold = null, float? fadeIn = null)
    {
        if (_isFading) return;

        StartCoroutine(FadeRoutine(
            onBlack,
            fadeOut ?? _defaultFadeOutDuration,
            hold ?? _defaultHoldDuration,
            fadeIn ?? _defaultFadeInDuration
        ));
    }

    private IEnumerator FadeRoutine(Action onBlack, float fadeOut, float hold, float fadeIn)
    {
        _isFading = true;

        if (_fadeCanvasGroup != null)
            _fadeCanvasGroup.blocksRaycasts = true;
        
        // Fade Out (To Black)
        float elapsed = 0f;
        while (elapsed < fadeOut)
        {
            elapsed += Time.unscaledDeltaTime;
            if (_fadeCanvasGroup != null)
                _fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeOut);
            yield return null;
        }

        if (_fadeCanvasGroup != null)
            _fadeCanvasGroup.alpha = 1f;

        // Execute transition
        onBlack?.Invoke();

        // Hold Black Screen
        if (hold > 0f)
            yield return new WaitForSecondsRealtime(hold);

        // Fade In (Back to Clear)
        elapsed = 0f;
        while (elapsed < fadeIn)
        {
            elapsed += Time.unscaledDeltaTime;
            if (_fadeCanvasGroup != null)
                _fadeCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeIn));
            yield return null;
        }

        if (_fadeCanvasGroup != null)
        {
            _fadeCanvasGroup.alpha = 0f;
            _fadeCanvasGroup.blocksRaycasts = false;
        }

        _isFading = false;
    }

}
