using UnityEngine;
using System;
using Patterns.Observer.EventManager_Delegates;

public class SoulEnergyManager : MonoBehaviour
{
    public static SoulEnergyManager Instance {get; private set;}

     [SerializeField] private int _maxSoulEnergyValue;
      
    public int MaxSoulEnergyValue
    {
        get
        {
          return _maxSoulEnergyValue;
        }
        private set
        {
          _maxSoulEnergyValue = value;
        }
    }

private int _currentSoulEnergy;
public int CurrentSoulEnergy
{
    get
    { 
      return _currentSoulEnergy;
    }

    private set
    {
        _currentSoulEnergy = value < 0 ? 0 : value;
    }
}


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void AddSoulEnergy(int AmountToAdd)
    {   if (AmountToAdd <= 0) return;
        else
        {
          var Plus = CurrentSoulEnergy + AmountToAdd;
          var Add = Plus > MaxSoulEnergyValue? Plus = MaxSoulEnergyValue : Plus = CurrentSoulEnergy + AmountToAdd;
          CurrentSoulEnergy = Add;
        }
        EventManager.TriggerEvent(EventsType.Event_SoulEnergyChanged, CurrentSoulEnergy);
    }

    public void RemoveSoulEnergy(int AmountToRemove)
    {
      if (AmountToRemove <= 0) return;
      else
      {
        var Change = CurrentSoulEnergy >= AmountToRemove? CurrentSoulEnergy = CurrentSoulEnergy - AmountToRemove: CurrentSoulEnergy = CurrentSoulEnergy;
        CurrentSoulEnergy = Change;
      }
      EventManager.TriggerEvent(EventsType.Event_SoulEnergyChanged, CurrentSoulEnergy);
    }
}
