using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _clickScale = 0.92f;
    [SerializeField] private float _scaleSpeed = 10f;

    [Header("Text Brighten")]
    [SerializeField] private bool _enableTextBrighten = true;
    [SerializeField] private float _brightenAmount = 0.4f;

    [Header("SFX")]
    [SerializeField] private bool _enableHoverSFX = true;
    [SerializeField] private bool _enableClickSFX = true;

    private SFXManager _sfxManager;
    private TMP_Text _label;

    private Vector3 _baseScale;
    private Vector3 _targetScale;
    private Color _baseTextColor;
    private Color _baseBackgroundColor;
    private bool _isHovered;

    private void Awake()
    {
        _baseScale = transform.localScale;
        _targetScale = _baseScale;

        // Assume label is a TMP child of the button
        if (_label == null)
            _label = GetComponentInChildren<TMP_Text>();

        if (_label != null)
            _baseTextColor = _label.color;

    }

    private void Start()
    {
        _sfxManager = SFXManager.Instance;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * _scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        _targetScale = _baseScale * _hoverScale;

        if (_enableTextBrighten && _label != null)
            _label.color = new Color(
                Mathf.Min(_baseTextColor.r + _brightenAmount, 1f),
                Mathf.Min(_baseTextColor.g + _brightenAmount, 1f),
                Mathf.Min(_baseTextColor.b + _brightenAmount, 1f),
                _baseTextColor.a
            );

        /* if (_enableHoverSFX)
             _sfxManager.PlaySFX(SFXManager.SFXCategoryType.UI_Hover);*/
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        _targetScale = _baseScale;

        if (_enableTextBrighten && _label != null)
            _label.color = _baseTextColor;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Snap to click scale for punchy feel
        transform.localScale = _baseScale * _clickScale;
        _targetScale = _isHovered ? _baseScale * _hoverScale : _baseScale;

        /*if (_enableClickSFX)
            _sfxManager.PlaySFX(SFXManager.SFXCategoryType.UI_Click);*/
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _targetScale = _isHovered ? _baseScale * _hoverScale : _baseScale;
    }
}