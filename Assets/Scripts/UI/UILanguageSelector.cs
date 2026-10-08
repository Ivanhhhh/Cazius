using TMPro;
using UnityEngine;

public class UILanguageSelector : MonoBehaviour
{
    [Header("Language Dropdown")]
    [SerializeField] private TMP_Dropdown _languageDropdown;

    private void Start()
    {
        if (_languageDropdown != null)
            _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private void OnDestroy()
    {
        if (_languageDropdown != null)
            _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
    }

    private void OnLanguageChanged(int index)
    {
        switch (index)
        {
            case 0:
                SetLanguage(SystemLanguage.English);
                break;

            case 1:
                SetLanguage(SystemLanguage.Spanish);
                break;
        }
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