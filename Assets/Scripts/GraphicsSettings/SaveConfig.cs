using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class SaveConfig : MonoBehaviour
{
    public int savedqualitysettings;

    private const string PrefKey = "GraphicsQuality";

    public void Save(int Index)
    {
        PlayerPrefs.SetInt(PrefKey, Index);
        PlayerPrefs.Save();
    }

    public void Load()
    {
      savedqualitysettings = PlayerPrefs.GetInt(PrefKey,QualitySettings.GetQualityLevel());
    }

    public void Clamp()
    {
        savedqualitysettings = Mathf.Clamp(savedqualitysettings,0,QualitySettings.names.Length - 1);
    }
}
