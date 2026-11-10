using TMPro;
using UnityEngine;
using System.Collections.Generic;
public class SetQuality : MonoBehaviour,IQualitySetter
/*{
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
}*/
{
    private ILoad _loadConfig;
    private ISave _saveConfig;

    private void Awake()
    {
        _loadConfig = GetComponent<ILoad>();
        _saveConfig = GetComponent<ISave>();

        if (_loadConfig == null || _saveConfig == null)
        {
            Debug.LogError("SetQuality: SaveConfig must be on the same GameObject.", this);
            return;
        }

        int savedQuality = _loadConfig.Load();
        QualitySettings.SetQualityLevel(savedQuality, true);
    }

    public void QualitySetter(int index)
    {
        if (index < 0 || index >= QualitySettings.names.Length) return;

        QualitySettings.SetQualityLevel(index, true);
        _saveConfig?.Save(index);
    }
}

