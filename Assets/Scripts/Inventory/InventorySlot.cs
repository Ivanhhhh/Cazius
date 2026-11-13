using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
            _inventoryUI.ShowItemDescription(_itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_inventoryUI != null)
            _inventoryUI.ClearItemDescription();
    }

    // NUEVO: clic derecho = menú contextual
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (_itemData == null) return;
        if (UIInventoryTooltip.Instance == null) return;

        UIInventoryTooltip.Instance.Show(_itemData, (RectTransform)transform);
    }
}
