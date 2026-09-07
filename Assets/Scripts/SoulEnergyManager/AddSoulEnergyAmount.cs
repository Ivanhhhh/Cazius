using UnityEngine;

using Patterns.Observer.EventManager_Delegates;

public class AddSoulEnergyAmount : MonoBehaviour
{
    private static int _SoulEnergyAmount;

    public static int SoulEnergyAmount
    {
        get
        {
           return _SoulEnergyAmount; 
        }

        set
        {
            if (value < 0)
            {
              _SoulEnergyAmount  = 0;
            }

             else
            {
              _SoulEnergyAmount = value;
            }

            EventManager.TriggerEvent(EventsType.Event_UpdateSoulEnergy, _SoulEnergyAmount);
        }
    }
}
