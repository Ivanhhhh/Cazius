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

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool AddItem(ItemData item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory full!");
            return false;
        }

        ItemData clonedItem = Instantiate(item);
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
                // Ya no hacemos nada al "Usar" la munición manualmente.
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
            {
                totalAmmo += item.value;
            }
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
                {
                    items.RemoveAt(i);
                }

                if (amountExtracted >= amountNeeded)
                {
                    break;
                }
            }
        }

        if (amountExtracted > 0)
        {
            onInventoryChanged?.Invoke();
        }

        return amountExtracted;
    }
}