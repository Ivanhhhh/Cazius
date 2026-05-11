using UnityEngine;

public class InventorySystem : MonoBehaviour // Lo tiene inventory
{
    public bool HasKey { get; private set; }

    public void AddKey()
    {
        HasKey = true;
    }
}
