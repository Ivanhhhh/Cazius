using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioMixer mixer;
    [SerializeField] AudioMixerSnapshot revenge;
    [SerializeField] AudioMixerSnapshot gameplay;
    [SerializeField] int Timer;
    [SerializeField] int Time2;

    public void SetMusicVolume(float volume)
    {
        // Volume expected in dB, so convert from 0–1 range
        mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    /* Add this to Slider Object  
        settingsSlider.onValueChanged.AddListener((value) => {
            audioSettingsManager.SetSFXVolume(value);
        });  
    */

    public void GameplaySnapshot()
    {
        gameplay.TransitionTo(Timer);
    }

    public void RevengeSnapshot()
    {
        revenge.TransitionTo(Time2);
    }

}
