using TMPro;
using UnityEngine;
using System.Collections.Generic;
public class SetQuality : MonoBehaviour,IQualitySetter
{
    public ILoad _LoadConfig;

    public ISave _SaveConfig;

    public IClamp _ClampConfig;



   void Awake()
{
    _LoadConfig.Load();
    _ClampConfig.Clamp();
    QualitySetter(_LoadConfig.Load());
    
    //interfaces
    _LoadConfig = GetComponent<ILoad>();
    _SaveConfig = GetComponent<ISave>();
    _ClampConfig = GetComponent<IClamp>();
     //

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
