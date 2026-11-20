using System.Collections;
using UnityEngine;

public class BossDirectionalLight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light _directionalLight;

    [Header("Swap Phase")]
    [ColorUsage(true, false)]
    [SerializeField] private Color _swapPhaseColor = Color.red;
    [SerializeField] private float _swapPhaseIntensity = 1f;

    [Header("Die Phase")]
    [ColorUsage(true, false)]
    [SerializeField] private Color _dieColor = Color.white;
    [SerializeField] private float _dieIntensity = 0.5f;

    private Color _baseColor;
    private float _baseIntensity;
    private Coroutine _transitionCoroutine;

    private void Awake()
    {
        if (_directionalLight == null) return;

        _baseColor = _directionalLight.color;
        _baseIntensity = _directionalLight.intensity;
    }

    private void OnEnable()
    {
        if (BossFightManager.Instance == null) return;

        BossFightManager.Instance.SwapPhase += OnSwapPhase;
        BossFightManager.Instance.Die += OnBossDie;
    }

    private void OnSwapPhase()
    {
        StartTransition(_swapPhaseColor, _swapPhaseIntensity, BossFightManager.Instance.TransitionDuration);
    }

    private void OnBossDie()
    {
        StartTransition(_dieColor, _dieIntensity, BossFightManager.Instance.TransitionDuration);
    }

    private void StartTransition(Color targetColor, float targetIntensity, float duration)
    {
        if (_directionalLight == null) return;

        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(LerpLight(targetColor, targetIntensity, duration));
    }

    private IEnumerator LerpLight(Color targetColor, float targetIntensity, float duration)
    {
        Color startColor = _directionalLight.color;
        float startIntensity = _directionalLight.intensity;

        if (duration <= 0f)
        {
            _directionalLight.color = targetColor;
            _directionalLight.intensity = targetIntensity;
            _transitionCoroutine = null;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            _directionalLight.color = Color.Lerp(startColor, targetColor, t);
            _directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null;
        }

        _directionalLight.color = targetColor;
        _directionalLight.intensity = targetIntensity;
        _transitionCoroutine = null;
    }

    private void OnDisable()
    {
        if (BossFightManager.Instance != null)
        {
            BossFightManager.Instance.SwapPhase -= OnSwapPhase;
            BossFightManager.Instance.Die -= OnBossDie;
        }

        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }

        if (_directionalLight == null) return;

        _directionalLight.color = _baseColor;
        _directionalLight.intensity = _baseIntensity;
    }
}