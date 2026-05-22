using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        Refresh(); 
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
        Debug.Log("4. El evento llegó a la UI. isOpen: " + isOpen);
        
        if (isOpen) 
        {
            OpenInventory();
        }
        else 
        {
            Debug.Log("5. Ejecutando CloseInventory()...");
            CloseInventory(); 
        }
    }

    public void OpenInventory()
    {
        panel.SetActive(true);
        Refresh(); 
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

            // BUSCAMOS EL TEXTO DENTRO DEL PREFAB
            var amountText = slot.GetComponentInChildren<TextMeshProUGUI>(true);

            if (hasItem)
            {
                var item = Inventory.Instance.items[i];
                icon.sprite = item.icon;

                // --- NUEVA LÓGICA PARA EL TEXTO ---
                if (amountText != null)
                {
                    // Si es munición, mostramos la cantidad y prendemos el texto
                    if (item.itemType == ItemType.Ammo || item.itemType == ItemType.Scrap)
                    {
                        amountText.text = item.value.ToString();
                        amountText.gameObject.SetActive(true);
                    }
                    else
                    {
                        // Si es cura, apagamos el número (a menos que quieras hacer curas apilables después)
                        amountText.gameObject.SetActive(false);
                    }
                }
                // ----------------------------------

                int index = i; 
                slot.GetComponent<Button>().onClick.AddListener(() =>
                    Inventory.Instance.UseItem(Inventory.Instance.items[index])
                );
            }
            else
            {
                // Si el slot está vacío, nos aseguramos de apagar el texto
                if (amountText != null) amountText.gameObject.SetActive(false);
            }
        }
    }
}