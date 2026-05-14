using UnityEngine;

public class InventorySystem : MonoBehaviour // Lo tiene inventory
{
    public bool HasEdenKey { get; private set; }
    public bool HasPurgatoryKey { get; private set; }

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

}
