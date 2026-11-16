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
    public int maxLoreSlots = 12;

    [Header("Item Collections")]
    public List<ItemData> regularItems = new();
    public List<ItemData> keyItems = new();
    public List<ItemData> loreItems = new();

    [Header("World Drop")]
    [SerializeField] private Transform _dropSpawnPoint;

    public UnityEvent onInventoryChanged;
    public event Action<ItemData> OnItemAdded;

    public ItemData itemToAdd;
    public List<ItemData> AllItems
    {
        get
        {
            var combined = new List<ItemData>(regularItems);
            combined.AddRange(keyItems);
            combined.AddRange(loreItems);
            return combined;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (itemToAdd != null) AddItem(itemToAdd);
    }

    public int GetItemCountByName(string targetName)
    {
        int total = 0;
        string searchName = targetName.Trim();
        foreach (var item in regularItems) if (item != null && item.name.Trim() == searchName) total += item.value;
        foreach (var item in keyItems) if (item != null && item.name.Trim() == searchName) total += item.value;
        foreach (var item in loreItems) if (item != null && item.name.Trim() == searchName) total += item.value;
        return total;
    }

    public bool ConsumeItemByName(string targetName, int amountToConsume)
    {
        int amountLeft = amountToConsume;
        string searchName = targetName.Trim();

        void ConsumeFromList(List<ItemData> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null) continue;
                if (list[i].name.Trim() == searchName)
                {
                    int taken = Mathf.Min(list[i].value, amountLeft);
                    list[i].value -= taken;
                    amountLeft -= taken;
                    if (list[i].value <= 0) list.RemoveAt(i);
                    if (amountLeft <= 0) break;
                }
            }
        }

        ConsumeFromList(regularItems);
        if (amountLeft > 0) ConsumeFromList(keyItems);
        if (amountLeft > 0) ConsumeFromList(loreItems);

        if (amountLeft < amountToConsume) onInventoryChanged?.Invoke();
        return amountLeft <= 0;
    }

    public bool AddItem(ItemData item)
    {
        if (item == null) return false;

        List<ItemData> targetList;
        int maxSlots;
        if (item.isKeyItem) { targetList = keyItems; maxSlots = maxKeySlots; }
        else if (item.isLoreItem) { targetList = loreItems; maxSlots = maxLoreSlots; }
        else { targetList = regularItems; maxSlots = maxRegularSlots; }

        int maxStack = Mathf.Max(1, item.maxStackSize);
        int amountToAdd = Mathf.Max(1, item.value);
        bool anyAdded = false;

        // 1. Intentar apilar en ítems existentes
        for (int i = 0; i < targetList.Count && amountToAdd > 0; i++)
        {
            var existing = targetList[i];
            if (existing == null) continue;
            if (!IsSameStack(existing, item)) continue;
            if (existing.value >= maxStack) continue;

            int space = maxStack - existing.value;
            int toAdd = Mathf.Min(space, amountToAdd);
            existing.value += toAdd;
            amountToAdd -= toAdd;
            anyAdded = true;
        }

        // 2. Si sobra cantidad, buscar el primer hueco o agregar al final
        while (amountToAdd > 0)
        {
            // Buscamos si hay algún slot nulo (hueco) en la lista
            int emptyIndex = targetList.FindIndex(x => x == null);

            // Si no hay huecos y ya estamos en el límite máximo, el inventario está lleno
            if (emptyIndex < 0 && targetList.Count >= maxSlots)
            {
                Debug.Log("[Inventory] Slots llenos, no se pudo añadir todo.");
                break;
            }

            ItemData clone = Instantiate(item);
            clone.name = item.name;
            clone.value = Mathf.Min(maxStack, amountToAdd);
            amountToAdd -= clone.value;

            if (emptyIndex >= 0)
            {
                // Lo colocamos en el primer hueco libre (izquierda superior)
                targetList[emptyIndex] = clone;
            }
            else
            {
                // Si no hay huecos, lo agregamos al final
                targetList.Add(clone);
            }

            anyAdded = true;
        }

        if (!anyAdded) return false;
        onInventoryChanged?.Invoke();
        OnItemAdded?.Invoke(item);
        return true;
    }

    private static bool IsSameStack(ItemData a, ItemData b)
    {
        if (a == null || b == null) return false;
        if (!string.IsNullOrEmpty(a.itemID) && !string.IsNullOrEmpty(b.itemID)) return a.itemID == b.itemID;
        return a.name == b.name;
    }

    public void UseItem(ItemData item)
    {
        if (item == null) return;

        switch (item.itemType)
        {
            case ItemType.Heal:
                _playerHealthSystem.Heal(10);
                SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.Heal);
                item.value--;
                if (item.value <= 0) regularItems.Remove(item);
                onInventoryChanged?.Invoke();
                break;

            case ItemType.LegendarySandwich:
                _playerHealthSystem.Heal(100);
                SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.Heal);
                item.value--;
                if (item.value <= 0) regularItems.Remove(item);
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
            if (item != null && item.itemType == ItemType.Ammo) totalAmmo += item.value;
        return totalAmmo;
    }

    public int ConsumeAmmo(int amountNeeded)
    {
        int amountExtracted = 0;
        for (int i = regularItems.Count - 1; i >= 0; i--)
        {
            if (regularItems[i] == null) continue;
            if (regularItems[i].itemType == ItemType.Ammo)
            {
                int bulletsToTake = Mathf.Min(regularItems[i].value, amountNeeded - amountExtracted);
                regularItems[i].value -= bulletsToTake;
                amountExtracted += bulletsToTake;
                if (regularItems[i].value <= 0) regularItems.RemoveAt(i);
                if (amountExtracted >= amountNeeded) break;
            }
        }
        if (amountExtracted > 0) onInventoryChanged?.Invoke();
        return amountExtracted;
    }

    public bool HasItem(string itemID)
    {
        return regularItems.Exists(i => i != null && i.itemID == itemID)
            || keyItems.Exists(i => i != null && i.itemID == itemID)
            || loreItems.Exists(i => i != null && i.itemID == itemID);
    }

    public bool RemoveItem(string itemID)
    {
        ItemData item = regularItems.Find(i => i != null && i.itemID == itemID);
        if (item != null) { regularItems.Remove(item); onInventoryChanged?.Invoke(); return true; }

        item = keyItems.Find(i => i != null && i.itemID == itemID);
        if (item != null) { keyItems.Remove(item); onInventoryChanged?.Invoke(); return true; }

        item = loreItems.Find(i => i != null && i.itemID == itemID);
        if (item != null) { loreItems.Remove(item); onInventoryChanged?.Invoke(); return true; }

        Debug.LogWarning($"[Inventory] Item not found for removal: '{itemID}'");
        return false;
    }

    public bool RemoveItemAndSpawn(ItemData item, int amountToDrop = -1)
    {
        if (item == null) return false;
        if (item.isKeyItem || item.isLoreItem)
        {
            Debug.LogWarning("[Inventory] Solo se pueden dropear items Regular.");
            return false;
        }
        if (!regularItems.Contains(item))
        {
            Debug.LogWarning("[Inventory] El item no está en regularItems.");
            return false;
        }

        if (amountToDrop <= 0) amountToDrop = item.value;
        amountToDrop = Mathf.Min(amountToDrop, item.value);
        item.value -= amountToDrop;
        if (item.value <= 0) regularItems.Remove(item);

        if (item.worldPrefab != null && _dropSpawnPoint != null)
        {
            GameObject go = Instantiate(item.worldPrefab, _dropSpawnPoint.position, _dropSpawnPoint.rotation);
            PickableItem pickable = go.GetComponent<PickableItem>();
            if (pickable != null)
            {
                ItemData droppedData = Instantiate(item);
                droppedData.value = amountToDrop;
                droppedData.name = item.name;
                pickable.Setup(droppedData);
            }
            else Debug.LogWarning("[Inventory] El worldPrefab no tiene el script PickableItem.");
        }
        else if (item.worldPrefab == null) Debug.LogWarning($"[Inventory] '{item.name}' no tiene worldPrefab asignado.");
        else Debug.LogWarning("[Inventory] _dropSpawnPoint no está asignado.");

        onInventoryChanged?.Invoke();
        return true;
    }

    // === NUEVO: helpers de slot para el move mode ===

    /// <summary>Intercambia dos posiciones (null permitido, crece la lista si hace falta).</summary>
    public static void SwapSlots(List<ItemData> list, int a, int b)
    {
        if (list == null || a == b) return;
        while (list.Count <= Mathf.Max(a, b)) list.Add(null);
        (list[a], list[b]) = (list[b], list[a]);
    }

    /// <summary>Mueve el contenido de "from" a "to" (deja from como null).</summary>
    public static void MoveSlot(List<ItemData> list, int from, int to)
    {
        if (list == null || from == to) return;
        while (list.Count <= Mathf.Max(from, to)) list.Add(null);
        list[to] = list[from];
        list[from] = null;
    }

    /// <summary>Compacta la lista borrando los null del final (para no acumular slots vacíos).</summary>
    public static void TrimTrailingNulls(List<ItemData> list)
    {
        if (list == null) return;
        while (list.Count > 0 && list[list.Count - 1] == null) list.RemoveAt(list.Count - 1);
    }
}