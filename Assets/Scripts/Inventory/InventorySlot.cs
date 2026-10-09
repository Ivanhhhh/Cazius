using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemData _itemData;
    private InventoryUI _inventoryUI;

    public void Setup(ItemData itemData, InventoryUI ui)
    {
        _itemData = itemData;
        _inventoryUI = ui;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemData != null && _inventoryUI != null)
        {
            _inventoryUI.ShowItemDescription(_itemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_inventoryUI != null)
        {
            _inventoryUI.ClearItemDescription();
        }
    }

}
