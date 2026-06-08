using UnityEngine;
using UnityEngine.UI;

public class Paus : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        sfxSlider.onValueChanged.AddListener((value) => AudioSettingsManager.instance.SetSFXVolume(value));       

        musicSlider.onValueChanged.AddListener((value) => AudioSettingsManager.instance.SetMusicVolume(value));
    }
}