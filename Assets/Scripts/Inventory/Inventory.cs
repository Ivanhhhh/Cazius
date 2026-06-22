using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Player_HealthSystem _playerHealthSystem;
    public static Inventory Instance { get; private set; }
    public int maxSlots = 12;
    public List<ItemData> items = new();
    public UnityEvent onInventoryChanged;
    public ItemData itemToAdd;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        AddItem(itemToAdd);
    }

    public bool AddItem(ItemData item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory full!");
            return false;
        }
        ItemData clonedItem = Instantiate(item);
        clonedItem.name = item.name;
        items.Add(clonedItem);
        onInventoryChanged?.Invoke();
        return true;
    }

    public void UseItem(ItemData item)
    {
        switch (item.itemType)
        {
            case ItemType.Heal:
                _playerHealthSystem.Heal(10);
                SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.Heal);
                items.Remove(item);
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
        foreach (var item in items)
        {
            if (item.itemType == ItemType.Ammo)
                totalAmmo += item.value;
        }
        return totalAmmo;
    }

    public int ConsumeAmmo(int amountNeeded)
    {
        int amountExtracted = 0;
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].itemType == ItemType.Ammo)
            {
                int bulletsToTake = Mathf.Min(items[i].value, amountNeeded - amountExtracted);
                items[i].value -= bulletsToTake;
                amountExtracted += bulletsToTake;
                if (items[i].value <= 0)
                    items.RemoveAt(i);

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
        return items.Exists(i => i.itemID == itemID);
    }

    public bool RemoveItem(string itemID)
    {
        ItemData item = items.Find(i => i.itemID == itemID);
        if (item == null)
        {
            Debug.LogWarning($"[Inventory] Item not found for removal: '{itemID}'");
            return false;
        }
        items.Remove(item);
        onInventoryChanged?.Invoke();
        return true;
    }

    // --- Save system ---

    public string[] GetAllItemIDs()
    {
        var ids = new List<string>();
        foreach (var item in items)
        {
            if (!item.isKeyItem)
                ids.Add(item.itemID);
        }
        return ids.ToArray();
    }

    public string[] GetKeyItemIDs()
    {
        var ids = new List<string>();
        foreach (var item in items)
        {
            if (item.isKeyItem)
                ids.Add(item.itemID);
        }
        return ids.ToArray();
    }

    public void LoadSaveData(string[] inventoryIDs, string[] keyItemIDs, ItemRegistry registry)
    {
        items.Clear();

        if (inventoryIDs != null)
        {
            foreach (var id in inventoryIDs)
            {
                var itemData = registry.GetItemByID(id);
                if (itemData != null)
                    items.Add(Instantiate(itemData));
            }
        }

        if (keyItemIDs != null)
        {
            foreach (var id in keyItemIDs)
            {
                var itemData = registry.GetItemByID(id);
                if (itemData != null)
                    items.Add(Instantiate(itemData));
            }
        }

        onInventoryChanged?.Invoke();
    }
}
