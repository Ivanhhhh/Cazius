using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GraphicsSettings : MonoBehaviour
{

    // [SerializeField] private TMP_Dropdown qualityDropdown;

    private void Awake()
    {
        // int defaultLevel = QualitySettings.GetQualityLevel();
        
        // QualitySettings.SetQualityLevel(_SavedQualitySettings,true);
    }

    void Start()
    {
        //  if (!qualityDropdown)
        //  {
        //     Debug.LogError("GraphicsQualitySettings: asigná un TMP_Dropdown en el Inspector.", this);
        //     return;
        //  }

        //  var ListOfQualitys = new List<string>(QualitySettings.names);

        // qualityDropdown.ClearOptions();
        // qualityDropdown.AddOptions(ListOfQualitys);
        // qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        // qualityDropdown.RefreshShownValue();
        // qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    //  public void SetQuality(int Index)
    // {
    //     if (Index < 0 || Index >= QualitySettings.names.Length)
    //     {
    //         return;
    //     }

    //     else ApplyQuality(Index);

        
    // }

    // private void ApplyQuality(int Index)
    // {
    //   QualitySettings.SetQualityLevel(Index, true);
    //   PlayerPrefs.SetInt(PrefKey, Index);
    //   PlayerPrefs.Save();
    // }

    // private void OnDestroy()
    // {
    //   if (qualityDropdown != null) qualityDropdown.onValueChanged.RemoveListener(SetQuality);
    // }
}
