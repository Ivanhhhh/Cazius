using UnityEngine;

public class InventorySystem : MonoBehaviour // Lo tiene inventory
{
    [Header("Keys")]
    public bool HasEdenKey { get; private set; }
    public bool HasPurgatoryKey { get; private set; }

    [Header("Soul Energy")]
    public int CurrentSoulEnergy { get; private set; }
    public int MaxSoulEnergy { get; private set; }

    public void AddEdenKey()
    {
        HasEdenKey = true;
        Debug.Log("Has Eden Key");
    }
    public void AddPurgatoryKey()
    {
        HasPurgatoryKey = true;
        Debug.Log("Has Purgatory Key");
    }
    public void AddSoulEnergy()
    {
        CurrentSoulEnergy++;
        Debug.Log("Soul Energy" + CurrentSoulEnergy);
    }
    public void RemoveSoulEnergy()
    {
        if (CurrentSoulEnergy > 0)
        {
            CurrentSoulEnergy--;
            Debug.Log("Soul Energy has consumed" + CurrentSoulEnergy);
        }
    }

}
