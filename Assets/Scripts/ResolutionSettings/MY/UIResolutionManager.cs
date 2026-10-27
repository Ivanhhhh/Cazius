using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResolutionsManager : MonoBehaviour
{
     [SerializeField] TMP_Dropdown _resolutionDropdown; // es un selector dropdown de resolucion
    [SerializeField] TMP_Dropdown _fullscreenDropdown; // es un selector dropdown de fullsceren

    List<Resolution> _availableResolutions;   // la tabla de traducción (vacía al principio)

    List<string> DropDownResolutions = new List<string>();

    void Start()
    {
        ResolutionToDropdown();
        ScreenModeDropdown();
    }

    public void ResolutionToDropdown()
  {
    ObtainResolutions providerofresolutions = new ObtainResolutions();

    _availableResolutions = providerofresolutions.SetupResolutions();

      List<string> options = new List<string>();
        foreach (Resolution r in _availableResolutions)
        {
            options.Add(r.width + "x" + r.height);
        }

        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(options);
        PlaceResolutionOnStart(Screen.currentResolution.width, Screen.currentResolution.height);
  }


   public void ScreenModeDropdown()
  {
    //ObtainResolutions providerofresolutions = new ObtainResolutions();

    //_availableResolutions = providerofresolutions.SetupResolutions();

      List<string> options = new List<string>();
      options.Add("Full Screen");
      options.Add("borderless window");
      options.Add("Windowed");

        _fullscreenDropdown.ClearOptions();
        _fullscreenDropdown.AddOptions(options);
  }


   void PlaceResolutionOnStart(int width, int height)
    {
        for (int i = 0; i < _availableResolutions.Count; i++)  // si el width y height que le paso esta en la coleccion de avaible resolutions retorna i
        {

            if (_availableResolutions[i].width == width && _availableResolutions[i].height == height)
            {
              _resolutionDropdown.SetValueWithoutNotify(i);
              return;
            }

          
           
        }

        _resolutionDropdown.SetValueWithoutNotify(_availableResolutions.Count - 1);
    }

}