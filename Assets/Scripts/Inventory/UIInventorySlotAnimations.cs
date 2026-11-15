using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlotAnimations : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Image a la que se le aplicará el alpha. Si está vacío, se busca en este GameObject.")]
    [SerializeField] private Image targetImage;

    [Tooltip("Opcional: si se asigna, se usa el alpha del CanvasGroup en lugar del color de la Image " +
             "(recomendado si la imagen tiene hijos y querés afectarlos a todos).")]

    [Header("Alpha")]
    [Range(0f, 1f)] public float minAlpha = 0f;   // alpha cuando está "apagada"
    [Range(0f, 1f)] public float maxAlpha = 1f;   // alpha cuando está "encendida"

    [Header("Fade Speeds (segundos)")]
    [Min(0f)] public float fadeInDuration = 0.5f;  // minAlpha -> maxAlpha
    [Min(0f)] public float fadeOutDuration = 0.5f;  // maxAlpha -> minAlpha

    [Header("Hold Times (segundos)")]
    [Min(0f)] public float holdVisibleTime = 1f;  // tiempo visible antes de apagarse
    [Min(0f)] public float holdInvisibleTime = 1f;  // tiempo invisible antes de encenderse

    [Header("Options")]
    [Tooltip("Repeticiones: -1 = infinito, 1 = un solo ciclo, etc.")]
    public int loopCount = -1;

    [Tooltip("Si true, arranca visible (hold visible -> fade out). Si false, arranca invisible.")]
    public bool startVisible = true;

    [Tooltip("Reproducir automáticamente al activarse el GameObject.")]
    public bool playOnEnable = true;

    [Tooltip("Usa tiempo no escalado (útil si el juego se pausa).")]
    public bool useUnscaledTime = false;

    [Tooltip("Curva de easing del fade. Null = lineal.")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    /// <summary>Se dispara al terminar todas las repeticiones (si loopCount > 0).</summary>
    public event Action OnPulseCompleted;

    /// <summary>True solo mientras el bucle de pulso está corriendo.</summary>
    public bool IsRunning => _isRunning;

    private Coroutine _activeCoroutine;
    private Color _baseColor;
    private bool _isRunning;

    // ------------------------------------------------------------------ //
    // Unity
    // ------------------------------------------------------------------ //

    void Awake()
    {

        if (targetImage != null)
            _baseColor = targetImage.color;
    }

    void OnEnable()
    {
        if (playOnEnable) StartPulse();
    }

    void OnDisable()
    {
        StopPulse(resetToVisible: false);
    }

    // ------------------------------------------------------------------ //
    // API pública
    // ------------------------------------------------------------------ //

    /// <summary>Arranca el bucle de fade infinito (o loopCount ciclos).</summary>
    public void StartPulse()
    {
        if (_isRunning) return;
        StopActiveCoroutine();
        _activeCoroutine = StartCoroutine(PulseRoutine());
    }

    /// <summary>Detiene el comportamiento de pulso.</summary>
    /// <param name="resetToVisible">Si true, deja la imagen en maxAlpha al detener.</param>
    public void StopPulse(bool resetToVisible = true)
    {
        StopActiveCoroutine();
        _isRunning = false;

        if (resetToVisible)
            SetAlpha(minAlpha);
    }

    /// <summary>Muestra u oculta la imagen, con o sin transición.</summary>
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

    // ------------------------------------------------------------------ //
    // Internos
    // ------------------------------------------------------------------ //

    private IEnumerator PulseRoutine()
    {
        _isRunning = true;
        int cyclesDone = 0;

        // Estado inicial
        bool visible = startVisible;
        SetAlpha(visible ? maxAlpha : minAlpha);
        yield return Wait(visible ? holdVisibleTime : holdInvisibleTime);

        while (loopCount < 0 || cyclesDone < loopCount)
        {
            // Fade out
            yield return FadeRoutine(maxAlpha, minAlpha, fadeOutDuration);
            yield return Wait(holdInvisibleTime);

            // Fade in
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
            Color c = _baseColor;
            c.a = alpha;
            targetImage.color = c;
        }
    }


}
