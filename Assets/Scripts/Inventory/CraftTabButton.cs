using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CraftTabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private CraftManager _craftManager;
    [SerializeField] private TMP_Text _text;

    [SerializeField] private int _tabIndex;

    [SerializeField] private Vector3 _normalScale = Vector3.one;
    [SerializeField] private Vector3 _hoverScale = new Vector3(1.08f, 1.08f, 1f);
    [SerializeField] private Vector3 _selectedScale = new Vector3(1.15f, 1.15f, 1f);

    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hoverColor = Color.yellow;
    [SerializeField] private Color _selectedColor = new Color(1f, 0.65f, 0.25f);

    [SerializeField] private float _animationSpeed = 12f;

    private bool _isHovered;
    private bool _isSelected;

    private Vector3 _targetScale;
    private Color _targetColor;

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TMP_Text>();

        _targetScale = _normalScale;
        _targetColor = _normalColor;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            _targetScale,
            Time.unscaledDeltaTime * _animationSpeed
        );

        if (_text != null)
        {
            _text.color = Color.Lerp(
                _text.color,
                _targetColor,
                Time.unscaledDeltaTime * _animationSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;

        if (!_isSelected)
        {
            _targetScale = _hoverScale;
            _targetColor = _hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;

        RefreshVisualState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _craftManager.ActivateList(_tabIndex);
    }

    public void SetSelectedButton(bool selected)
    {
        _isSelected = selected;
        RefreshVisualState();
    }

    private void RefreshVisualState()
    {
        if (_isSelected)
        {
            _targetScale = _selectedScale;
            _targetColor = _selectedColor;
        }
        else if (_isHovered)
        {
            _targetScale = _hoverScale;
            _targetColor = _hoverColor;
        }
        else
        {
            _targetScale = _normalScale;
            _targetColor = _normalColor;
        }
    }
}