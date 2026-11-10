using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveConfig : MonoBehaviour,ISave,ILoad,IClamp
/*{
    public int savedqualitysettings;

    private const string PrefKey = "GraphicsQuality";

    public void Save(int Index)
    {
        PlayerPrefs.SetInt(PrefKey, Index);
        PlayerPrefs.Save();
    }

   public int Load()
   {
    int saved = PlayerPrefs.GetInt(PrefKey, QualitySettings.GetQualityLevel());
    return Mathf.Clamp(saved, 0, QualitySettings.names.Length - 1);
   }

    public void Clamp()
    {
        savedqualitysettings = Mathf.Clamp(savedqualitysettings,0,QualitySettings.names.Length - 1);
    }
}*/
{
    public int savedqualitysettings;

    private const string PrefKey = "GraphicsQuality";

    public void Save(int index)
    {
        if (index < 0 || index >= QualitySettings.names.Length) return;

        savedqualitysettings = index;
        PlayerPrefs.SetInt(PrefKey, index);
        PlayerPrefs.Save();
    }

    public int Load()
    {
        savedqualitysettings = PlayerPrefs.GetInt(PrefKey, GetDefaultQualityIndex());
        Clamp();
        return savedqualitysettings;
    }

    public void Clamp()
    {
        savedqualitysettings = Mathf.Clamp(savedqualitysettings, 0, QualitySettings.names.Length - 1);
    }

    private int GetDefaultQualityIndex()
    {
        string[] qualityNames = QualitySettings.names;

        for (int i = 0; i < qualityNames.Length; i++)
        {
            if (string.Equals(qualityNames[i], "PC_High", StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return Mathf.Max(0, qualityNames.Length - 1);
    }
}
