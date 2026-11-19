using System;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public class UISensitivityManager : MonoBehaviour
{

    [SerializeField] Slider _sensSlider;
    private const string SensPrefKey = "SensitivityMultiplier";

    public static event Action<float> OnSensitivityChanged;

    private void Start()
    {
        GameManager.Instance.sensitivityManager = this;

        float sensMult = PlayerPrefs.GetFloat(SensPrefKey, 1f);

        _sensSlider.value = sensMult;

        _sensSlider.onValueChanged.AddListener(SetSensitivityMult);  
    }

    public static float GetSensitivityMult()
    {
        return PlayerPrefs.GetFloat(SensPrefKey, 1f);
    }

    public void SetSensitivityMult(float mult)
    {
        PlayerPrefs.SetFloat(SensPrefKey, mult);

        OnSensitivityChanged?.Invoke(mult);
    }
}
