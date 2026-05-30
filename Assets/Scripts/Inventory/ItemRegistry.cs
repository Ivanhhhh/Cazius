using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRegistry", menuName = "Inventory/Item Registry")]
public class ItemRegistry : ScriptableObject
{
    [SerializeField] private List<ItemData> items = new();

    private Dictionary<string, ItemData> _lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<string, ItemData>();

        foreach (var item in items)
        {
            if (item == null) continue;

            if (string.IsNullOrEmpty(item.itemID))
            {
                Debug.LogWarning($"[ItemRegistry] Item '{item.name}' has no itemID set. Skipping.");
                continue;
            }

            if (!_lookup.TryAdd(item.itemID, item))
                Debug.LogWarning($"[ItemRegistry] Duplicate itemID found: '{item.itemID}'. Skipping.");
        }
    }

    public ItemData GetItemByID(string itemID)
    {
        if (_lookup == null) BuildLookup();

        if (_lookup.TryGetValue(itemID, out var item))
            return item;

        Debug.LogWarning($"[ItemRegistry] Item not found for ID: '{itemID}'");
        return null;
    }

    public bool Contains(string itemID)
    {
        if (_lookup == null) BuildLookup();
        return _lookup.ContainsKey(itemID);
    }
}