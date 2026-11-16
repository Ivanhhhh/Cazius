using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ItemData _itemData;
    private InventoryUI _inventoryUI;
    private int _slotIndex = -1;

    public void Setup(ItemData itemData, InventoryUI ui, int slotIndex)
    {
        _itemData = itemData;
        _inventoryUI = ui;
        _slotIndex = slotIndex;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_inventoryUI == null) return;

        if (_inventoryUI.IsMoveMode)
        {
            _inventoryUI.SetSlotHover(_slotIndex, true);
            return;
        }

        if (_itemData != null) _inventoryUI.ShowItemDescription(_itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_inventoryUI == null) return;

        if (_inventoryUI.IsMoveMode)
        {
            _inventoryUI.SetSlotHover(_slotIndex, false);
            return;
        }

        _inventoryUI.ClearItemDescription();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_inventoryUI == null) return;

        // Clic derecho: Abrir Tooltip
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_inventoryUI.IsMoveMode) return;
            if (_itemData == null) return;
            if (UIInventoryTooltip.Instance == null) return;

            UIInventoryTooltip.Instance.Show(_itemData, (RectTransform)transform);
            return;
        }

        // Clic izquierdo: Intercambiar o Usar
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_inventoryUI.IsMoveMode)
            {
                _inventoryUI.HandleSlotClicked(_slotIndex);
                return;
            }

            if (_itemData != null && Inventory.Instance != null)
                Inventory.Instance.UseItem(_itemData);
        }
    }
}