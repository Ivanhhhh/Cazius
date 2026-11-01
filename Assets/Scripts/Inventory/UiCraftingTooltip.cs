using GLTFast.Schema;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiCraftingTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ItemData _itemType;
    [SerializeField] private Vector2 _tooltipOffset = new Vector2(100f, 0f);
    public void OnPointerEnter(PointerEventData eventData)
    {
        string description = GameManager.Instance.inventoryUI.ReturnItemDescription(_itemType);
        UITooltip.Instance.Show(description, GetComponent<RectTransform>(), _tooltipOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITooltip.Instance.Hide();
    }
}
