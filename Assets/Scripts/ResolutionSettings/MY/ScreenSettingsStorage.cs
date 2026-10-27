using UnityEngine;

public class ScreenSettingsStorage
{  
    const string WidthKey  = "ResolutionWidth";
    const string HeightKey = "ResolutionHeight";
    const string ModeKey   = "ScreenMode";

    public void SaveWidthScreen(int widthValue)
    {
        PlayerPrefs.SetInt(WidthKey,widthValue);
        PlayerPrefs.Save();
    }

    public void SaveHeightScreen(int heightValue)
    {
        PlayerPrefs.SetInt(HeightKey,heightValue);
        PlayerPrefs.Save();
    }

    public void SaveScreenMode(FullScreenMode mode)
    {
        PlayerPrefs.SetInt(ModeKey, (int)mode);
        PlayerPrefs.Save();
    }

   public int LoadWidth()
   {
    return PlayerPrefs.GetInt(WidthKey, Screen.currentResolution.width);
   }

   public int LoadHeight()
   {
    return PlayerPrefs.GetInt(HeightKey, Screen.currentResolution.height);
   }

   public FullScreenMode LoadScreenMode()
   {
    int saved = PlayerPrefs.GetInt(ModeKey, (int)FullScreenMode.FullScreenWindow);
    return (FullScreenMode)saved;
   }

}
