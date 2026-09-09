using UnityEngine;
using Patterns.Observer.EventManager_Delegates;

public class Discard : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.SubscribeToEvent(EventsType.Event_SoulEnergyChanged, OnSoulEnergyChanged);
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeToEvent(EventsType.Event_SoulEnergyChanged, OnSoulEnergyChanged);
    }

    private void OnSoulEnergyChanged(params object[] parameters)
    {
        long newValue = (long)parameters[0];
        Debug.LogError("Evento recibido, energía actual: " + newValue + "AAAAAAAAA");
    }
}