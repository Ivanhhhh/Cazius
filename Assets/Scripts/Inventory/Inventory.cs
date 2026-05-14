using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Player_HealthSystem _playerHealthSystem;
    [SerializeField] private Player_AimAndShoot _playerAimAndShoot;

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

        items.Add(item);
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
                break;
            case ItemType.Ammo:
                _playerAimAndShoot.AddReserveBullets(20);
                SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.RechargingGun);
                break;
        }

        items.Remove(item);
        onInventoryChanged?.Invoke();
    }
}