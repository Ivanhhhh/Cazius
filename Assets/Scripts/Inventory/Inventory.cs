using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Player_HealthSystem _playerHealthSystem;
    public static Inventory Instance { get; private set; }

    [Header("Slot Limits")]
    public int maxRegularSlots = 12;
    public int maxKeySlots = 12;

    [Header("Item Collections")]
    public List<ItemData> regularItems = new();
    public List<ItemData> keyItems = new();

    public UnityEvent onInventoryChanged;
    public event Action<ItemData> OnItemAdded;

    public ItemData itemToAdd;
    public List<ItemData> AllItems
    {
        get
        {
            var combined = new List<ItemData>(regularItems);
            combined.AddRange(keyItems);
            return combined;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (itemToAdd != null)
        {
            AddItem(itemToAdd);
        }
    }

    public int GetItemCountByName(string targetName)
    {
        int total = 0;
        string searchName = targetName.Trim();

        foreach (var item in regularItems)
        {
            if (item.name.Trim() == searchName)
                total += item.value;
        }

        foreach (var item in keyItems)
        {
            if (item.name.Trim() == searchName)
                total += item.value;
        }

        return total;
    }

    public bool ConsumeItemByName(string targetName, int amountToConsume)
    {
        int amountLeft = amountToConsume;
        string searchName = targetName.Trim();

        // Helper to consume items from a specific target list
        void ConsumeFromList(List<ItemData> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].name.Trim() == searchName)
                {
                    int taken = Mathf.Min(list[i].value, amountLeft);
                    list[i].value -= taken;
                    amountLeft -= taken;

                    if (list[i].value <= 0)
                        list.RemoveAt(i);

                    if (amountLeft <= 0)
                        break;
                }
            }
        }

        // Tries regular items first, then key items if needed
        ConsumeFromList(regularItems);
        if (amountLeft > 0)
            ConsumeFromList(keyItems);

        if (amountLeft < amountToConsume)
            onInventoryChanged?.Invoke();

        return amountLeft <= 0;
    }

    public bool AddItem(ItemData item)
    {
        if (item == null) return false;

        if (item.isKeyItem)
        {
            if (keyItems.Count >= maxKeySlots)
            {
                Debug.Log("Key Item Inventory full!");
                return false;
            }

            ItemData clonedItem = Instantiate(item);
            clonedItem.name = item.name;
            keyItems.Add(clonedItem);
        }
        else
        {
            if (regularItems.Count >= maxRegularSlots)
            {
                Debug.Log("Regular Inventory full!");
                return false;
            }

            ItemData clonedItem = Instantiate(item);
            clonedItem.name = item.name;
            regularItems.Add(clonedItem);
        }

        onInventoryChanged?.Invoke();
        OnItemAdded?.Invoke(item);
        return true;
    }

    public void UseItem(ItemData item)
    {
        if (item == null) return;

        switch (item.itemType)
        {
            case ItemType.Heal:
                _playerHealthSystem.Heal(10);
                SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.Heal);
                regularItems.Remove(item);
                onInventoryChanged?.Invoke();
                break;

            case ItemType.LegendarySandwich:
                _playerHealthSystem.Heal(100);
                SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.Heal);
                SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.Heal);
                regularItems.Remove(item);
                onInventoryChanged?.Invoke();
                break;

            case ItemType.Ammo:
                Debug.Log("La munición se recarga automáticamente con la tecla R.");
                break;
        }
    }

    public int GetTotalAmmo()
    {
        int totalAmmo = 0;
        foreach (var item in regularItems)
        {
            if (item.itemType == ItemType.Ammo)
                totalAmmo += item.value;
        }
        return totalAmmo;
    }

    public int ConsumeAmmo(int amountNeeded)
    {
        int amountExtracted = 0;
        for (int i = regularItems.Count - 1; i >= 0; i--)
        {
            if (regularItems[i].itemType == ItemType.Ammo)
            {
                int bulletsToTake = Mathf.Min(regularItems[i].value, amountNeeded - amountExtracted);
                regularItems[i].value -= bulletsToTake;
                amountExtracted += bulletsToTake;

                if (regularItems[i].value <= 0)
                    regularItems.RemoveAt(i);

                if (amountExtracted >= amountNeeded)
                    break;
            }
        }

        if (amountExtracted > 0)
            onInventoryChanged?.Invoke();

        return amountExtracted;
    }

    // --- Quest system ---

    public bool HasItem(string itemID)
    {
        return regularItems.Exists(i => i.itemID == itemID) || keyItems.Exists(i => i.itemID == itemID);
    }

    public bool RemoveItem(string itemID)
    {
        ItemData item = regularItems.Find(i => i.itemID == itemID);
        if (item != null)
        {
            regularItems.Remove(item);
            onInventoryChanged?.Invoke();
            return true;
        }

        item = keyItems.Find(i => i.itemID == itemID);
        if (item != null)
        {
            keyItems.Remove(item);
            onInventoryChanged?.Invoke();
            return true;
        }

        Debug.LogWarning($"[Inventory] Item not found for removal: '{itemID}'");
        return false;
    }

}
