using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResolutionsManager : MonoBehaviour
{
    ScreenSettingsStorage _storage = new ScreenSettingsStorage();
    ScreenSettingsApplier _applier = new ScreenSettingsApplier();

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
        PlaceResolutionOnStart(_storage.LoadWidth(), _storage.LoadHeight());
        _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
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

        _fullscreenDropdown.SetValueWithoutNotify(ScreenModeToIndex(_storage.LoadScreenMode()));
        _fullscreenDropdown.onValueChanged.AddListener(OnFullscreenChanged);
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

    int ScreenModeToIndex(FullScreenMode mode)
{
    switch (mode)
    {
        case FullScreenMode.ExclusiveFullScreen: return 0;
        case FullScreenMode.FullScreenWindow:    return 1;
        case FullScreenMode.Windowed:            return 2;
        default:                                 return 1;
    }
}

FullScreenMode IndexToScreenMode(int index)
{
    switch (index)
    {
        case 0:  return FullScreenMode.ExclusiveFullScreen;
        case 1:  return FullScreenMode.FullScreenWindow;
        case 2:  return FullScreenMode.Windowed;
        default: return FullScreenMode.FullScreenWindow;
    }
}

void OnResolutionChanged(int index)
{
    if (index < 0 || index >= _availableResolutions.Count)
        return;

    Resolution resolution = _availableResolutions[index];

    _storage.SaveWidthScreen(resolution.width);
    _storage.SaveHeightScreen(resolution.height);
    _applier.Apply(resolution.width, resolution.height, _storage.LoadScreenMode());
}

void OnFullscreenChanged(int index)
{
    FullScreenMode mode = IndexToScreenMode(index);

    _storage.SaveScreenMode(mode);
    _applier.Apply(_storage.LoadWidth(), _storage.LoadHeight(), mode);
}

void OnDestroy()
{
    if (_resolutionDropdown != null)
        _resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);

    if (_fullscreenDropdown != null)
        _fullscreenDropdown.onValueChanged.RemoveListener(OnFullscreenChanged);
}

}