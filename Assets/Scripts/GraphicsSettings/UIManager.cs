using TMPro;
using UnityEngine;
using System.Collections.Generic;
public class UIManager : MonoBehaviour
{
     [SerializeField] private TMP_Dropdown _qualityDropdown;

    public IQualitySetter _Quality;
    public ISave _SaveConfig;

    public ILoad _LoadConfig;

    void Awake()
    {
            //interfaces
      _LoadConfig = GetComponent<ILoad>();
      _SaveConfig = GetComponent<ISave>();
      _Quality = GetComponent<IQualitySetter>();
          /////
    }

    void Start()
    {
        _LoadConfig.Load();
         if (!_qualityDropdown)
        {
             Debug.LogError("GraphicsQualitySettings: asigná un TMP_Dropdown en el Inspector.", this);
             return;
        }

          var ListOfQualitys = new List<string>(QualitySettings.names);

         _qualityDropdown.ClearOptions();
         _qualityDropdown.AddOptions(ListOfQualitys);
         _qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
         _qualityDropdown.RefreshShownValue();
         _qualityDropdown.onValueChanged.AddListener(_Quality.QualitySetter);
         _qualityDropdown.onValueChanged.AddListener(_SaveConfig.Save);

    }

    private void OnDestroy()
     {
       if (_qualityDropdown != null) 
       {
         _qualityDropdown.onValueChanged.RemoveListener(_Quality.QualitySetter);
         _qualityDropdown.onValueChanged.RemoveListener(_SaveConfig.Save); 
       }     
     }
}
