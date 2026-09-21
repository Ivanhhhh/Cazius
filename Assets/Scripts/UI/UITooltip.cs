using System.Collections;
using TMPro;
using UnityEngine;

public class UITooltip : MonoBehaviour
{
    public static UITooltip Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private TMP_Text _tooltipText;
    [SerializeField] private RectTransform _tooltipRect;

    [Header("Offset")]
    [SerializeField] private Vector2 _offset = new Vector2(250f, 0f);

    [Header("Settings")]
    [SerializeField] private float _animDuration = 0.15f;
    [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine _animCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _tooltipPanel.SetActive(false);
        _tooltipRect.localScale = Vector3.zero;
    }

    public void Show(string text, RectTransform targetRect)
    {
        _tooltipText.text = text;

        Vector3 targetPos = targetRect.position
                          + (targetRect.right * (_offset.x * targetRect.lossyScale.x))
                          + (targetRect.up * (_offset.y * targetRect.lossyScale.y));

        _tooltipRect.position = targetPos;
        _tooltipRect.rotation = targetRect.rotation;

        _tooltipPanel.SetActive(true);
        _tooltipRect.localScale = Vector3.zero;

        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _animCoroutine = StartCoroutine(AnimateScale(Vector3.zero, Vector3.one));
    }

    public void Hide()
    {
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _tooltipPanel.SetActive(false);
        _tooltipRect.localScale = Vector3.zero;
    }

    private IEnumerator AnimateScale(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < _animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = _scaleCurve.Evaluate(Mathf.Clamp01(elapsed / _animDuration));
            _tooltipRect.localScale = Vector3.LerpUnclamped(from, to, t);
            yield return null;
        }
        _tooltipRect.localScale = to;
    }

}
