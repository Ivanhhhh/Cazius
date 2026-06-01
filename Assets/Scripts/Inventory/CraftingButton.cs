using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public struct CraftingIngredient
{
    public string itemName; // AHORA PIDE EL NOMBRE DEL SCRIPTABLE OBJECT
    public int requiredAmount; 
}

public class CraftingButton : MonoBehaviour
{
    [Header("Crafting Recipe")]
    public ItemData itemToCraft; 
    public List<CraftingIngredient> ingredients; 

    [Header("UI References")]
    public Button craftButton;
    public Image buttonImage; 

    void Awake()
    {
        if (craftButton == null) Debug.LogError("[Crafting] ERROR: Falta asignar el craftButton en el Inspector.", gameObject);
        if (buttonImage == null) Debug.LogError("[Crafting] ERROR: Falta asignar la buttonImage en el Inspector.", gameObject);

        craftButton.onClick.AddListener(CraftItem);
    }

    void OnEnable()
    {
        Debug.Log($"[Crafting] OnEnable ejecutado en el botón para craftear: {(itemToCraft != null ? itemToCraft.name : "NADA")}");

        if (Inventory.Instance != null)
        {
            Inventory.Instance.onInventoryChanged.AddListener(UpdateCraftState);
            UpdateCraftState(); 
        }
        else
        {
            Debug.LogError("[Crafting] ERROR: Inventory.Instance es NULL. ¿Hay un inventario en la escena?");
        }
    }

    void OnDisable()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.onInventoryChanged.RemoveListener(UpdateCraftState);
        }
    }

    public void UpdateCraftState()
    {
        Debug.Log("[Crafting] Actualizando estado visual del botón...");
        bool canCraft = HasAllIngredients();

        Color currentColor = buttonImage.color;

        if (canCraft)
        {
            Debug.Log("[Crafting] -> Tienes todos los materiales. Botón ENCENDIDO.");
            craftButton.interactable = true;
            currentColor.a = 1f; 
        }
        else
        {
            Debug.Log("[Crafting] -> Faltan materiales. Botón APAGADO.");
            craftButton.interactable = false;
            currentColor.a = 0.4f; 
        }

        buttonImage.color = currentColor;
    }

    private bool HasAllIngredients()
    {
        Debug.Log("--- [Crafting] Iniciando chequeo de ingredientes ---");

        foreach (CraftingIngredient req in ingredients)
        {
            int totalInInventory = 0;

            foreach (ItemData item in Inventory.Instance.items)
            {
                // Agregamos un pequeño fix visual: quitamos espacios extra por si acaso
                string currentItemName = item.name.Trim();
                string requiredItemName = req.itemName.Trim();

                if (currentItemName == requiredItemName) 
                {
                    totalInInventory += item.value; 
                }
            }

            Debug.Log($"[Crafting] Chequeando '{req.itemName}': Tienes {totalInInventory} / Necesitas {req.requiredAmount}");

            if (totalInInventory < req.requiredAmount)
            {
                Debug.Log($"[Crafting] FALLO: No alcanza con el ítem '{req.itemName}'.");
                return false;
            }
        }

        Debug.Log("--- [Crafting] ÉXITO: Tienes todos los ingredientes ---");
        return true;
    }

    public void CraftItem()
    {
        Debug.Log("[Crafting] Botón presionado. Intentando craftear...");

        if (!HasAllIngredients())
        {
            Debug.LogWarning("[Crafting] Intento de crafteo bloqueado: Faltan ingredientes (Esto no debería pasar si el botón está apagado).");
            return; 
        }

        foreach (CraftingIngredient req in ingredients)
        {
            ConsumeIngredient(req.itemName, req.requiredAmount);
        }

        Debug.Log($"[Crafting] Crafteo exitoso. Agregando '{itemToCraft.name}' al inventario.");
        Inventory.Instance.AddItem(itemToCraft);
    }

    private void ConsumeIngredient(string targetItemName, int amountToConsume)
    {
        int amountLeft = amountToConsume;
        Debug.Log($"[Crafting] Consumiendo {amountToConsume} de '{targetItemName}'...");

        for (int i = Inventory.Instance.items.Count - 1; i >= 0; i--)
        {
            ItemData item = Inventory.Instance.items[i];

            if (item.name.Trim() == targetItemName.Trim())
            {
                int taken = Mathf.Min(item.value, amountLeft);
                item.value -= taken;
                amountLeft -= taken;

                Debug.Log($"[Crafting] Restados {taken} de '{targetItemName}'. Quedan por restar: {amountLeft}");

                if (item.value <= 0)
                {
                    Debug.Log($"[Crafting] El slot de '{targetItemName}' quedó vacío. Borrándolo del inventario.");
                    Inventory.Instance.items.RemoveAt(i);
                }

                if (amountLeft <= 0) 
                {
                    Debug.Log($"[Crafting] Terminado de consumir '{targetItemName}'.");
                    break; 
                }
            }
        }
    }
}