using UnityEngine;

public class ScreenSettingsStorage : MonoBehaviour
{  
    const string WidthKey  = "ResolutionWidth";
    const string HeightKey = "ResolutionHeight";
    const string ModeKey   = "ScreenMode";

    public void SaveWidthScreen(int WidthValue)
    {
        PlayerPrefs.SetInt(WidthKey,WidthValue);
        PlayerPrefs.Save();
    }

    public void SaveHeightScreen(int HeightValue)
    {
        PlayerPrefs.SetInt(HeightKey,HeightValue);
        PlayerPrefs.Save();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
