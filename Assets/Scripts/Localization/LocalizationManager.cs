using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private SystemLanguage _currentLanguage = SystemLanguage.English;
    [SerializeField] private DataLocalization[] _localizationData;

    private Dictionary<SystemLanguage, Dictionary<string, string>> _translations = new();

    public SystemLanguage CurrentLanguage
    {
        get => _currentLanguage;
        set => _currentLanguage = value;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Process and parse all the loaded language files into memory
        if (_localizationData != null && _localizationData.Length > 0)
        {
            _translations = LanguageU.LoadTranslate(_localizationData);
        }
    }

    public string GetTranslate(string id)
    {
        if (!_translations.ContainsKey(_currentLanguage))
            return $"[No Lang: {_currentLanguage}]";

        if (!_translations[_currentLanguage].ContainsKey(id))
            return $"[Missing ID: {id}]";

        return _translations[_currentLanguage][id];
    }

    public void ChangeLanguage(SystemLanguage newLanguage)
    {
        _currentLanguage = newLanguage;

        ButtonTranslate[] allTexts = FindObjectsByType<ButtonTranslate>(FindObjectsSortMode.None);
        foreach (ButtonTranslate textComp in allTexts)
        {
            textComp.ApplyTranslation();
        }
    }

}
