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
    public bool isLoreItem;
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public string displayName; // Name displayed in the inventory UI
    public string itemDescription;
    [Header("Quantity")]
    [Tooltip("Cantidad que se suma al recogerlo. Para munición = balas; para el resto normalmente 1.")]
    public int value = 1;

    [Tooltip("Tamaño máximo de pila. 1 = no apilable.")]
    [Min(1)] public int maxStackSize = 1;
    [Header("World Drop")]
    [Tooltip("Prefab spawneado al soltar el item. Solo se usa si es Regular.")]
    public GameObject worldPrefab;
}