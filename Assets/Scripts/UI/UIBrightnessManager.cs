using UnityEngine;
using UnityEngine.UI;

public class UIBrightnessManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider _brightnessSlider;

    private const string BrightnessPrefKey = "BrightnessOffset";

    private static readonly int BrightnessID =
        Shader.PropertyToID("_BrightnessOffset");

    private void Awake()
    {
        float brightness = PlayerPrefs.GetFloat(
            BrightnessPrefKey, 0f);

        ApplyBrightness(brightness);

        if (_brightnessSlider != null)
        {
            _brightnessSlider.SetValueWithoutNotify(brightness);

            _brightnessSlider.onValueChanged.AddListener(
                SetBrightness);
        }
    }

    public void SetBrightness(float brightness)
    {
        ApplyBrightness(brightness);

        PlayerPrefs.SetFloat(BrightnessPrefKey, brightness);
    }

    private void ApplyBrightness(float brightness)
    {
        Shader.SetGlobalFloat(BrightnessID, brightness);
    }

    public float GetBrightness()
    {
        return Shader.GetGlobalFloat(BrightnessID);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        if (_brightnessSlider != null)
        {
            _brightnessSlider.onValueChanged.RemoveListener(
                SetBrightness);
        }
    }
}