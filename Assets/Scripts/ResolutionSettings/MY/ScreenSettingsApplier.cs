using UnityEngine;

public class ScreenSettingsApplier : MonoBehaviour
{

    public void Apply(int width, int height, FullScreenMode mode)
    {
        Screen.SetResolution(width, height, mode);
    }

}
