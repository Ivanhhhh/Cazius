using UnityEngine;

public enum ItemType
{
    Heal,
    Ammo,
    Scrap,
    Teddybear,
    Herbs,
    WorldCupAlbum,
    EdenKey,
    PurgatoryKey,
    DepotKey,
    Football,
    LegendarySandwich,
    Recorder,
    Sauce,
    Bread
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
    public string displayName; // Name displayed in the inventory UI
    public string itemDescription;
}