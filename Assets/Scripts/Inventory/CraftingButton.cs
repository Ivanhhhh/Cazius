using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public struct CraftingIngredient
{
    public ItemType itemType; 
    public int requiredAmount; 
}

public class CraftingButton : MonoBehaviour
{
    [Header("Crafting Recipe")]
    public ItemData itemToCraft; 
    public List<CraftingIngredient> ingredients; 

    [Header("UI References")]
    public Button craftButton;
    // NUEVO: Referencia directa a la imagen del botón en lugar del Canvas Group
    public Image buttonImage; 

    void Start()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.onInventoryChanged.AddListener(UpdateCraftState);
            UpdateCraftState(); 
        }
        
        craftButton.onClick.AddListener(CraftItem);
    }

    void OnDestroy()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.onInventoryChanged.RemoveListener(UpdateCraftState);
        }
    }

    public void UpdateCraftState()
    {
        bool canCraft = HasAllIngredients();

        // 1. Guardamos el color actual de la imagen
        Color currentColor = buttonImage.color;

        if (canCraft)
        {
            craftButton.interactable = true;
            currentColor.a = 1f; // Opacidad total (100%)
        }
        else
        {
            craftButton.interactable = false;
            currentColor.a = 0.4f; // Semitransparente (40%)
        }

        // 2. Le aplicamos el color modificado de vuelta a la imagen
        buttonImage.color = currentColor;
    }

    private bool HasAllIngredients()
    {
        foreach (CraftingIngredient req in ingredients)
        {
            int totalInInventory = 0;

            foreach (ItemData item in Inventory.Instance.items)
            {
                if (item.itemType == req.itemType)
                {
                    totalInInventory += item.value; 
                }
            }

            if (totalInInventory < req.requiredAmount)
            {
                return false;
            }
        }

        return true;
    }

    public void CraftItem()
    {
        if (!HasAllIngredients()) return; 

        foreach (CraftingIngredient req in ingredients)
        {
            ConsumeIngredient(req.itemType, req.requiredAmount);
        }

        Inventory.Instance.AddItem(itemToCraft);
    }

    private void ConsumeIngredient(ItemType type, int amountToConsume)
    {
        int amountLeft = amountToConsume;

        for (int i = Inventory.Instance.items.Count - 1; i >= 0; i--)
        {
            ItemData item = Inventory.Instance.items[i];

            if (item.itemType == type)
            {
                int taken = Mathf.Min(item.value, amountLeft);
                item.value -= taken;
                amountLeft -= taken;

                if (item.value <= 0)
                {
                    Inventory.Instance.items.RemoveAt(i);
                }

                if (amountLeft <= 0) break; 
            }
        }
    }
}