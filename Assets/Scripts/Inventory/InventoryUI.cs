using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public Transform slotContainer;   // The GridLayoutGroup parent
    public GameObject slotPrefab;

    void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.onInventoryChanged.AddListener(Refresh);

        InventoryInputHandler.OnInventoryToggled += OnToggled;
        Refresh(); // runs directly when the panel activates
    }

    void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.onInventoryChanged.RemoveListener(Refresh);

        InventoryInputHandler.OnInventoryToggled -= OnToggled;
    }

    void OnDestroy()
    {
        InventoryInputHandler.OnInventoryToggled -= OnToggled;
    }

    void OnToggled(bool isOpen)
    {
        if (isOpen) Refresh();
    }

    public void OpenInventory()
    {
        panel.SetActive(true);
        Refresh(); // Build slots only when actually opening
    }

    public void CloseInventory()
    {
        panel.SetActive(false);
    }


    void Refresh()
    {
        if (Inventory.Instance == null) return;

        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        // Always spawn all 12 slots
        for (int i = 0; i < Inventory.Instance.maxSlots; i++)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            bool hasItem = i < Inventory.Instance.items.Count;

            var icon = slot.GetComponent<Image>();
            icon.enabled = hasItem;

            if (hasItem)
            {
                var item = Inventory.Instance.items[i];
                icon.sprite = item.icon;

                int index = i;
                slot.GetComponent<Button>().onClick.AddListener(() =>
                    Inventory.Instance.UseItem(Inventory.Instance.items[index])
                );
            }
        }
    }
}