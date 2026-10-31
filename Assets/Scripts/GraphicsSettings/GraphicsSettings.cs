using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GraphicsSettings : MonoBehaviour
{
     private const string PrefKey = "GraphicsQuality";
     private int _SavedQualitySettings;

    [SerializeField] private TMP_Dropdown qualityDropdown;

    private void Awake()
    {
        int defaultLevel = QualitySettings.GetQualityLevel();
        _SavedQualitySettings = PlayerPrefs.GetInt(PrefKey,defaultLevel);
        _SavedQualitySettings = Mathf.Clamp(_SavedQualitySettings,0,QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(_SavedQualitySettings,true);
    }

    void Start()
    {
         if (!qualityDropdown)
         {
            Debug.LogError("GraphicsQualitySettings: asigná un TMP_Dropdown en el Inspector.", this);
            return;
         }

         var ListOfQualitys = new List<string>(QualitySettings.names);

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(ListOfQualitys);
        qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        qualityDropdown.RefreshShownValue();
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }
}
