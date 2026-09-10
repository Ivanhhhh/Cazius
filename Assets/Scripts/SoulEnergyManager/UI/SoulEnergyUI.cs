using UnityEngine;
using TMPro;
using Patterns.Observer.EventManager_Delegates;

public class SoulEnergyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _soulEnergyText; // el texto que se ve en pantalla

    private void OnEnable()
    {
        EventManager.SubscribeToEvent(EventsType.Event_SoulEnergyChanged, OnSoulEnergyChanged);

        UpdateText(SoulEnergyManager.Instance.CurrentSoulEnergy);
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeToEvent(EventsType.Event_SoulEnergyChanged, OnSoulEnergyChanged);
    }

    private void OnSoulEnergyChanged(params object[] parameters)
    {
        int newValue = (int)parameters[0];
        UpdateText(newValue);
    }

    private void UpdateText(int value)
    {
        _soulEnergyText.text = "Soul Energy:  "+value.ToString();
    }
}