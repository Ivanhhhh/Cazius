
using UnityEngine;

public enum ItemType
{
    Heal,
    Ammo,
    Scrap,
    Cat,
    Herbs,
    WorldCupAlbum,
    DepotKey
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public bool isKeyItem;
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int value; // HP restored, Ammo added, Soul Energy, etc
}