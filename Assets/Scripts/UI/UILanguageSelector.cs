using UnityEngine;
using UnityEngine.UI;

public class UILanguageSelector : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button _englishButton;
    [SerializeField] private Button _spanishButton;

    private void Start()
    {
        if (_englishButton != null)
            _englishButton.onClick.AddListener(() => SetLanguage(SystemLanguage.English));

        if (_spanishButton != null)
            _spanishButton.onClick.AddListener(() => SetLanguage(SystemLanguage.Spanish));
    }

    private void OnDestroy()
    {
        if (_englishButton != null) _englishButton.onClick.RemoveAllListeners();
        if (_spanishButton != null) _spanishButton.onClick.RemoveAllListeners();
    }

    private void SetLanguage(SystemLanguage language)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.ChangeLanguage(language);
            Debug.Log($"Language switched to: {language}");
        }
    }
}
