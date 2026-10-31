using TMPro;
using UnityEngine;
using System.Collections.Generic;
public class SetQuality : MonoBehaviour
{
    public UIManager _uimanager;
    public SaveConfig _SaveConfig;

   void Awake()
{
    _SaveConfig.Load();
    _SaveConfig.Clamp();
    QualitySetter(_SaveConfig.savedqualitysettings);
}

    public void QualitySetter(int Index)
    {
        if (Index < 0 || Index >= QualitySettings.names.Length)
        {
            return;
        }

        else ApplyQuality(Index);       
    }

      private void ApplyQuality(int Index)
      {
        QualitySettings.SetQualityLevel(Index, true);   
      }    
}
