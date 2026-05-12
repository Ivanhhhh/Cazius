using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public Transform slotContainer;   // The GridLayoutGroup parent
    public GameObject slotPrefab;     // See setup below

    void Start()
    {
        Inventory.Instance.onInventoryChanged.AddListener(Refresh);
        panel.SetActive(false);
        Refresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            panel.SetActive(!panel.activeSelf);
    }

    void Refresh()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        // Always spawn all 12 slots
        for (int i = 0; i < Inventory.Instance.maxSlots; i++)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            bool hasItem = i < Inventory.Instance.items.Count;

            var icon = slot.transform.Find("Icon").GetComponent<Image>();
            icon.enabled = hasItem;

            if (hasItem)
            {
                var item = Inventory.Instance.items[i];
                icon.sprite = item.icon;

                int index = i; // capture for lambda
                slot.GetComponent<Button>().onClick.AddListener(() =>
                    Inventory.Instance.UseItem(Inventory.Instance.items[index])
                );
            }
        }
    }
}