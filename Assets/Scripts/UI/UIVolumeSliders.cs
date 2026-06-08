using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VolumeSliders : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private SFXManager _sfxManager;

    private void Start()
    {
        _sfxManager = SFXManager.Instance;

        // Load saved values or default to 0.75
        // EXPOSE VARS IN AUDIOMIXER
        float master = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        _masterSlider.value = master;
        _musicSlider.value = music;
        _sfxSlider.value = sfx;

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);

        // Listeners
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Listen for pointer release on SFX slider
        SliderPointerUpHandler sfxPointerUp = _sfxSlider.gameObject.AddComponent<SliderPointerUpHandler>();
        sfxPointerUp.OnPointerUpEvent += OnSFXSliderReleased;
    }

    private void OnSFXSliderReleased()
    {
        _sfxManager.PlaySFX(SFXManager.SFXCategoryType.ClickButton);
    }

    public void SetMasterVolume(float value)
    {
        _audioMixer.SetFloat("MasterVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        _audioMixer.SetFloat("MusicVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        _audioMixer.SetFloat("SFXVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private float LinearToDecibel(float linear)
    {
        return linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
    }
}

public class SliderPointerUpHandler : MonoBehaviour, IPointerUpHandler
{
    public event Action OnPointerUpEvent;

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke();
    }
}