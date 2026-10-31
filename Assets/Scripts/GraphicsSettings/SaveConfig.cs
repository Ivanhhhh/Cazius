using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class SaveConfig : MonoBehaviour,ISave,ILoad,IClamp
{
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
}
