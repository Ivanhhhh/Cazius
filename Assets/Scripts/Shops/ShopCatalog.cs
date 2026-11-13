using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopItemEntry
{
    public ItemData item;
    [Min(0)] public int price = 10;
    public bool buyOnce = false;
}

[CreateAssetMenu(fileName = "NewShopCatalog", menuName = "Shop/Shop Catalog")]
public class ShopCatalog : ScriptableObject
{
    [Header("Shop Identity")]
    public string shopTitleID;

    [Header("Items For Sale")]
    public List<ShopItemEntry> items = new();
}
