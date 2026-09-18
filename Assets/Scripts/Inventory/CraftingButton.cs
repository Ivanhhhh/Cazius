using Patterns.Observer.EventManager_Delegates;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [Tooltip("Costo en Soul Energy para craftear. 0 = no consume.")]
    public int soulEnergyCost = 30;

    [Header("UI References")]
    public Button craftButton;
    public Image buttonImage;
    public List<TextMeshProUGUI> _ingredientsAvailableTextList_;
    void Awake()
    {
        if (craftButton == null) Debug.LogError("[Crafting] ERROR: Falta asignar el craftButton en el Inspector.", gameObject);
        if (buttonImage == null) Debug.LogError("[Crafting] ERROR: Falta asignar la buttonImage en el Inspector.", gameObject);

        craftButton.onClick.AddListener(CraftItem);
    }

    void OnEnable()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.onInventoryChanged.AddListener(UpdateCraftState);
            UpdateCraftState();
        }

        // Nuevo: escuchar cambios de energía
        EventManager.SubscribeToEvent(EventsType.Event_SoulEnergyChanged, OnSoulEnergyChanged);
    }

    void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.onInventoryChanged.RemoveListener(UpdateCraftState);

        EventManager.UnsubscribeToEvent(EventsType.Event_SoulEnergyChanged, OnSoulEnergyChanged);
    }
    private void OnSoulEnergyChanged(params object[] p)
    {
        UpdateCraftState();
    }

    public void UpdateCraftState()
    {
        Debug.Log("[Crafting] Actualizando estado visual del botón...");
        bool canCraft = HasAllIngredients() && HasEnoughSoulEnergy(); 

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
            currentColor.a = 0.1f; 
        }

        buttonImage.color = currentColor;

        UpdateIngredientsTextList();
    }

    private int GetItemCount(string itemName)
    {
        int totalInInventory = 0;

        foreach (ItemData item in Inventory.Instance.items)
        {
            string currentItemName = item.name.Trim();
            string requiredItemName = itemName.Trim();

            if (currentItemName == requiredItemName)
            {
                totalInInventory += item.value;
            }
        }

        return totalInInventory;
    }

    private void UpdateIngredientsTextList()
    {
        // Verificar que la lista de textos no esté vacía
        if (_ingredientsAvailableTextList_ == null || _ingredientsAvailableTextList_.Count == 0)
        {
            Debug.LogWarning("[Crafting] No hay TextMeshProUGUI asignados en la lista ingredientsTextList.");
            return;
        }

        // Recorrer la lista de ingredientes
        for (int i = 0; i < ingredients.Count; i++)
        {
            // Verificar que exista un texto en el mismo índice
            if (i < _ingredientsAvailableTextList_.Count && _ingredientsAvailableTextList_[i] != null)
            {
                // Obtener la cantidad actual en el inventario
                int totalInInventory = GetItemCount(ingredients[i].itemName);
                int requiredAmount = ingredients[i].requiredAmount;

                // Actualizar el texto con el formato que prefieras
                _ingredientsAvailableTextList_[i].text = $"{totalInInventory}/{requiredAmount}";

                Debug.Log($"[Crafting] Texto[{i}] actualizado: {ingredients[i].itemName} = {totalInInventory}/{requiredAmount}");
            }
            else
            {
                Debug.LogWarning($"[Crafting] No hay TextMeshProUGUI asignado en el índice {i} de ingredientsTextList.");
            }
        }
    }
    private bool HasEnoughSoulEnergy()
    {
        if (soulEnergyCost <= 0) return true;
        if (SoulEnergyManager.Instance == null) return false;

        return SoulEnergyManager.Instance.CurrentSoulEnergy >= soulEnergyCost;
    }
    private bool HasAllIngredients()
    {
        Debug.Log("--- [Crafting] Iniciando chequeo de ingredientes ---");

        foreach (CraftingIngredient req in ingredients)
        {
            int totalInInventory = GetItemCount(req.itemName);

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
        if (!HasAllIngredients())
        {
            Debug.LogWarning("[Crafting] Faltan ingredientes.");
            return;
        }

        if (!HasEnoughSoulEnergy())
        {
            Debug.LogWarning("[Crafting] No hay suficiente Soul Energy.");
            return;
        }

        // Consumir ingredientes
        foreach (CraftingIngredient req in ingredients)
            ConsumeIngredient(req.itemName, req.requiredAmount);

        // Consumir Soul Energy
        if (soulEnergyCost > 0 && SoulEnergyManager.Instance != null)
            SoulEnergyManager.Instance.RemoveSoulEnergy(soulEnergyCost);

        // Craftear
        Inventory.Instance.AddItem(itemToCraft);
        Debug.Log($"[Crafting] Crafteado '{itemToCraft.name}'. -{soulEnergyCost} Soul Energy.");
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