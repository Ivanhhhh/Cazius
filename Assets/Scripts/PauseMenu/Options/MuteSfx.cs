using UnityEngine;
using UnityEngine.UI;


public class MuteSfx : MonoBehaviour
{
     Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => AudioSettingsManager.instance.SetSFXVolume(-80f));

    }

}
