using GLTFast.Schema;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiCraftingTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ItemData _itemType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        string description = GameManager.Instance.inventoryUI.ReturnItemDescription(_itemType);
        UITooltip.Instance.Show(description, GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITooltip.Instance.Hide();
    }
}
