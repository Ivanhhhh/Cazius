using UnityEngine;

public class ScreenSettingsApplier
{

    public void Apply(int width, int height, FullScreenMode mode)
    {
        Screen.SetResolution(width, height, mode);
    }

}
