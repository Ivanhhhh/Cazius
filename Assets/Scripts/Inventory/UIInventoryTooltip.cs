using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventoryTooltip : MonoBehaviour
{
    public static UIInventoryTooltip Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private RectTransform _tooltipRect;
    [SerializeField] private Button _dropButton;
    [SerializeField] private Button _moveButton;

    [Header("Offset (Posición respecto al Slot)")]
    [SerializeField] private Vector2 _offset = new Vector2(50f, -50f);

    [Header("Animation")]
    [SerializeField] private float _animDuration = 0.15f;
    [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private ItemData _currentItem;
    private Coroutine _animCoroutine;
    private bool _isOpen;
    private int _showFrame;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (_tooltipRect != null)
        {
            _tooltipRect.pivot = new Vector2(0f, 1f);
            _tooltipRect.localScale = Vector3.zero;
        }

        _tooltipPanel.SetActive(false);

        if (_dropButton != null) _dropButton.onClick.AddListener(OnDropClicked);
        if (_moveButton != null) _moveButton.onClick.AddListener(OnMoveClicked);
    }

    private void OnDestroy()
    {
        if (_dropButton != null) _dropButton.onClick.RemoveListener(OnDropClicked);
        if (_moveButton != null) _moveButton.onClick.RemoveListener(OnMoveClicked);
    }

    public void Show(ItemData item, RectTransform slotRect)
    {
        if (item == null || slotRect == null) return;

        // Si estaba cerrándose, cancelamos esa animación
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);

        _currentItem = item;
        _showFrame = Time.frameCount;
        _isOpen = true;

        if (_dropButton != null)
            _dropButton.gameObject.SetActive(!item.isKeyItem && !item.isLoreItem);

        _tooltipPanel.SetActive(true);

        Vector3 targetPos = slotRect.position
                          + (slotRect.right * (_offset.x * slotRect.lossyScale.x))
                          + (slotRect.up * (_offset.y * slotRect.lossyScale.y));

        _tooltipRect.position = targetPos;
        _tooltipRect.rotation = slotRect.rotation;

        // Animación de apertura
        _tooltipRect.localScale = Vector3.zero;
        _animCoroutine = StartCoroutine(AnimateScale(Vector3.zero, Vector3.one, false));
    }

    public void Hide()
    {
        // Si ya está cerrado o cerrándose, no hacemos nada
        if (!_isOpen) return;

        _currentItem = null;
        _isOpen = false;

        // Cancelamos cualquier animación previa
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);

        // Iniciamos la animación de cierre
        _animCoroutine = StartCoroutine(AnimateScale(_tooltipRect.localScale, Vector3.zero, true));
    }

    private void Update()
    {
        if (!_isOpen || _tooltipRect == null) return;
        if (Time.frameCount == _showFrame) return;

        bool clicked = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                       (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame);

        if (clicked)
        {
            bool overTooltip = IsPointerOverTooltip();
            bool overSlot = IsPointerOverSlot();

            if (!overTooltip && !overSlot)
            {
                Hide();
            }
        }
    }

    private bool IsPointerOverTooltip()
    {
        if (EventSystem.current == null) return false;

        Vector2 mousePos = Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = mousePos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject.transform.IsChildOf(_tooltipRect))
                return true;
        }
        return false;
    }

    private bool IsPointerOverSlot()
    {
        return false;
    }

    // Agregamos un parámetro "deactivateOnFinish"
    private IEnumerator AnimateScale(Vector3 from, Vector3 to, bool deactivateOnFinish)
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

        // Si era una animación de cierre, apagamos el panel al terminar
        if (deactivateOnFinish && _tooltipPanel != null)
        {
            _tooltipPanel.SetActive(false);
        }

        _animCoroutine = null;
    }

    private void OnDropClicked()
    {
        if (_currentItem == null) { Hide(); return; }

        Inventory.Instance.RemoveItemAndSpawn(_currentItem, _currentItem.value);
        Hide();
    }

    private void OnMoveClicked()
    {
        Hide();
    }
}