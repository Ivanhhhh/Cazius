using TMPro;
using UnityEngine;

public class ButtonTranslate : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("If empty uses Game Object name as ID")]
    [SerializeField] private string _localizationID = string.Empty;
    [Tooltip("If empty auto fetches component")]
    [SerializeField] private TextMeshProUGUI _textUI;

    private void Awake()
    {
        if (_textUI == null)
        {
            _textUI = GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        ApplyTranslation();
    }

    private void Start()
    {
        ApplyTranslation();
    }

    public void ApplyTranslation()
    {
        if (_textUI == null) return;

        if (string.IsNullOrEmpty(_localizationID))
        {
            _localizationID = gameObject.name;
        }

        if (LocalizationManager.Instance != null)
        {
            _textUI.text = LocalizationManager.Instance.GetTranslate(_localizationID);
        }
    }

}
