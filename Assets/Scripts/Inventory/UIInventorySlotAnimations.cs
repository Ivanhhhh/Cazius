using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlotAnimations : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Image a la que se le aplicará el alpha. Si está vacío, se busca en este GameObject.")]
    [SerializeField] private Image targetImage;

    [Header("Alpha")]
    [Range(0f, 1f)] public float minAlpha = 0f;
    [Range(0f, 1f)] public float maxAlpha = 1f;

    [Header("Fade Speeds (segundos)")]
    [Min(0f)] public float fadeInDuration = 0.5f;
    [Min(0f)] public float fadeOutDuration = 0.5f;

    [Header("Hold Times (segundos)")]
    [Min(0f)] public float holdVisibleTime = 1f;
    [Min(0f)] public float holdInvisibleTime = 1f;

    [Header("Options")]
    public int loopCount = -1;
    public bool startVisible = true;
    public bool playOnEnable = true;
    public bool useUnscaledTime = false;

    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public event Action OnPulseCompleted;
    public bool IsRunning => _isRunning;

    private Coroutine _activeCoroutine;
    private bool _isRunning;

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    void OnEnable()
    {
        if (playOnEnable) StartPulse();
    }

    void OnDisable()
    {
        StopPulse(resetToVisible: false);
    }

    public void StartPulse()
    {
        if (_isRunning) return;
        StopActiveCoroutine();
        _activeCoroutine = StartCoroutine(PulseRoutine());
    }

    public void StopPulse(bool resetToVisible = true)
    {
        StopActiveCoroutine();
        _isRunning = false;

        if (resetToVisible)
            SetAlpha(minAlpha);
    }

    public void SetVisible(bool visible, bool instant = true)
    {
        StopActiveCoroutine();
        _isRunning = false;

        float target = visible ? maxAlpha : minAlpha;

        if (instant)
            SetAlpha(target);
        else
            _activeCoroutine = StartCoroutine(
                FadeRoutine(GetCurrentAlpha(), target, visible ? fadeInDuration : fadeOutDuration));
    }

    private IEnumerator PulseRoutine()
    {
        _isRunning = true;
        int cyclesDone = 0;

        bool visible = startVisible;
        SetAlpha(visible ? maxAlpha : minAlpha);
        yield return Wait(visible ? holdVisibleTime : holdInvisibleTime);

        while (loopCount < 0 || cyclesDone < loopCount)
        {
            yield return FadeRoutine(maxAlpha, minAlpha, fadeOutDuration);
            yield return Wait(holdInvisibleTime);

            yield return FadeRoutine(minAlpha, maxAlpha, fadeInDuration);
            yield return Wait(holdVisibleTime);

            cyclesDone++;
        }

        _isRunning = false;
        _activeCoroutine = null;
        OnPulseCompleted?.Invoke();
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        if (duration <= 0f || Mathf.Approximately(from, to))
        {
            SetAlpha(to);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += GetDeltaTime();
            float normalized = Mathf.Clamp01(t / duration);
            float eased = fadeCurve != null ? fadeCurve.Evaluate(normalized) : normalized;
            SetAlpha(Mathf.Lerp(from, to, eased));
            yield return null;
        }

        SetAlpha(to);
    }

    private IEnumerator Wait(float seconds)
    {
        if (seconds <= 0f) yield break;
        float t = 0f;
        while (t < seconds)
        {
            t += GetDeltaTime();
            yield return null;
        }
    }

    private void StopActiveCoroutine()
    {
        if (_activeCoroutine != null)
        {
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
        }
    }

    private float GetDeltaTime() => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    private float GetCurrentAlpha()
    {
        if (targetImage != null) return targetImage.color.a;
        return 1f;
    }

    private void SetAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);

        if (targetImage != null)
        {
            // SOLO CAMBIAMOS EL ALPHA, PRESERVAMOS EL COLOR RGB DEL PREFAB
            Color c = targetImage.color;
            c.a = alpha;
            targetImage.color = c;
        }
    }
}
