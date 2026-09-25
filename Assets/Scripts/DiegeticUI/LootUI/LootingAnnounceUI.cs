using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootingAnnounceUI : MonoBehaviour
{
    [Header("Item UI")]
    [SerializeField] private SpriteRenderer _itemIconRenderer;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _itemAmountText;

    [Header("MeshRenderers (fondo + barras, todos con _OpacityMultiplier)")]
    [SerializeField] private MeshRenderer[] _meshRenderers;

    [Header("Fade")]
    [SerializeField] private float _fadeDuration = 0.2f;
    [SerializeField] private float _holdDuration = 2f;
    [SerializeField] private float _hiddenVertexOffset = 0.2f;
    [SerializeField] private float _visibleVertexOffset = 0f;

    [Header("Queue")]
    [SerializeField] private bool _useQueue = true;

    private TMP_Text[] _allTexts;
    private readonly Queue<ItemData> _pending = new();
    private Coroutine _routine;
    private bool _isShowing;

    // ====== LIFECYCLE ======

    private void Awake()
    {
        var list = new List<TMP_Text> { _itemNameText, _itemAmountText };
        list.RemoveAll(t => t == null);
        _allTexts = list.ToArray();

        ApplyAlpha(0f);
    }

    private void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnItemAdded += Enqueue;
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnItemAdded -= Enqueue;
    }

    // ====== ENTRADA ======

    private void Enqueue(ItemData item)
    {
        if (item == null) return;

        if (_useQueue)
        {
            _pending.Enqueue(item);
            if (!_isShowing)
                _routine = StartCoroutine(ProcessQueue());
        }
        else
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(ShowOnce(item));
        }
    }

    private IEnumerator ProcessQueue()
    {
        _isShowing = true;
        while (_pending.Count > 0)
        {
            var item = _pending.Dequeue();
            yield return ShowOnce(item);
        }
        _isShowing = false;
        _routine = null;
    }

    // ====== MOSTRAR ======

    private IEnumerator ShowOnce(ItemData item)
    {
        if (_itemIconRenderer != null) _itemIconRenderer.sprite = item.icon;
        if (_itemNameText != null) _itemNameText.text = item.itemName;
        if (_itemAmountText != null) _itemAmountText.text = "x" + item.value;

        yield return FadeAll(0f, 1f, _fadeDuration);
        yield return new WaitForSecondsRealtime(_holdDuration);
        yield return FadeAll(1f, 0f, _fadeDuration);
    }

    // ====== FADES ======

    private IEnumerator FadeAll(float start, float end, float duration)
    {
        float elapsed = 0f;
        ApplyAlpha(start);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float value = Mathf.Lerp(start, end, elapsed / duration);
            ApplyAlpha(value);
            yield return null;
        }

        ApplyAlpha(end);
    }

    private void ApplyAlpha(float value)
    {
        // Textos TMP
        foreach (var t in _allTexts)
            t.alpha = value;

        // Icono (SpriteRenderer)
        if (_itemIconRenderer != null)
        {
            var c = _itemIconRenderer.color;
            c.a = value;
            _itemIconRenderer.color = c;
        }

        // MeshRenderers (fondo + barras)
        if (_meshRenderers != null)
        {
            foreach (var mr in _meshRenderers)
            {
                if (mr == null) continue;
                var mat = mr.material;

                if (mat.HasProperty("_OpacityMultiplier"))
                    mat.SetFloat("_OpacityMultiplier", value);

                if (mat.HasProperty("_VertexOffset"))
                {
                    float vertex = Mathf.Lerp(_hiddenVertexOffset, _visibleVertexOffset, value);
                    mat.SetFloat("_VertexOffset", vertex);
                }
            }
        }
    }
}