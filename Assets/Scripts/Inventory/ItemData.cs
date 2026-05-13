using UnityEngine;

public enum ItemType
{
    Heal,
    Ammo
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int value; // HP restored OR ammo added
}