using UnityEngine;

public class KeyInventorySystem : MonoBehaviour // Lo tiene Inventory
{
    [Header("Keys")]
    public bool HasEdenKey { get; private set; }
    public bool HasPurgatoryKey { get; private set; }

    [Header("Soul Energy")]
    public int CurrentSoulEnergy { get; private set; }
    public int MaxSoulEnergy { get; private set; }

    public static KeyInventorySystem Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddEdenKey()
    {
        HasEdenKey = true;
    }
    public void AddPurgatoryKey()
    {
        HasPurgatoryKey = true;
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
