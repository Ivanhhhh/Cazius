using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ShopItemEntry _entry;
    private ShopUI _shopUI;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(HandleClick);
    }

    public void Setup(ShopItemEntry entry, ShopUI ui)
    {
        _entry = entry;
        _shopUI = ui;
        _button.interactable = (_entry != null && _entry.item != null);
    }

    private void HandleClick()
    {
        if (_entry != null && _shopUI != null)
            _shopUI.TryPurchase(_entry);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_entry != null && _entry.item != null && _shopUI != null)
            _shopUI.ShowItemDescription(_entry);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_shopUI != null)
            _shopUI.ClearItemDescription();
    }

}
