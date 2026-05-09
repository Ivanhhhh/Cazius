using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public bool HasKey { get; private set; }

    public void AddKey()
    {
        HasKey = true;
    }
}
