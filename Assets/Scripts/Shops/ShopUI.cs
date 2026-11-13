using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("Panel & Slots")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Button _closeButton;

    [Header("Header & Currency (Optional)")]
    [SerializeField] private TMP_Text _shopTitleText;
    [SerializeField] private TMP_Text _soulEnergyText;

    [Header("Description Box & Typewriter")]
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private float _typewriterCharDelay = 0.02f;

    [Header("Audio (Optional)")]
    [SerializeField] private SFXManager.SFXCategoryType _purchaseSFX = SFXManager.SFXCategoryType.HoverButton;

    private ShopCatalog _currentCatalog;
    private Action<bool> _onShopClosed;
    private bool _purchasedAnything;
    private Coroutine _typewriterCoroutine;

    public bool IsOpen => _panel != null && _panel.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (_closeButton != null)
            _closeButton.onClick.AddListener(CloseShop);

        if (_panel != null)
            _panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(CloseShop);
    }

    // --- Public API ---

    public void OpenShop(ShopCatalog catalog, Action<bool> onShopClosed = null)
    {
        if (catalog == null) return;

        _currentCatalog = catalog;
        _onShopClosed = onShopClosed;
        _purchasedAnything = false;

        if (_shopTitleText != null)
        {
            _shopTitleText.text = !string.IsNullOrEmpty(catalog.shopTitleID)
                ? LocalizationManager.Instance.GetTranslate(catalog.shopTitleID)
                : string.Empty;
        }

        if (GameInputManager.Instance != null)
            GameInputManager.Instance.EnableShopInput();

        if (SoulEnergyManager.Instance != null)
            SoulEnergyManager.Instance.RequestShowUI(this);

        _panel.SetActive(true);
        ClearItemDescription();
        UpdateCurrencyDisplay();
        Refresh();
    }

    public void CloseShop()
    {
        if (_panel != null)
            _panel.SetActive(false);

        ClearItemDescription();

        if (SoulEnergyManager.Instance != null)
            SoulEnergyManager.Instance.RequestHideUI(this);

        if (GameInputManager.Instance != null)
            GameInputManager.Instance.DisableShopInput();

        var callback = _onShopClosed;
        bool madePurchase = _purchasedAnything;

        _currentCatalog = null;
        _onShopClosed = null;

        callback?.Invoke(madePurchase);
    }

    // --- Purchase Logic ---

    public void TryPurchase(ShopItemEntry entry)
    {
        if (entry == null || entry.item == null) return;
        if (SoulEnergyManager.Instance == null || Inventory.Instance == null) return;

        if (SoulEnergyManager.Instance.CurrentSoulEnergy < entry.price)
        {
            Debug.Log("[ShopUI] Not enough Soul Energy.");
            return;
        }

        bool added = Inventory.Instance.AddItem(entry.item);
        if (!added)
        {
            Debug.Log("[ShopUI] Inventory full.");
            return;
        }

        SoulEnergyManager.Instance.RemoveSoulEnergy(entry.price);
        _purchasedAnything = true;

        if (SFXManager.Instance != null) SFXManager.Instance.PlaySFX(_purchaseSFX);

        UpdateCurrencyDisplay();
        Refresh();
    }

    private void UpdateCurrencyDisplay()
    {
        if (_soulEnergyText != null && SoulEnergyManager.Instance != null)
            _soulEnergyText.text = SoulEnergyManager.Instance.CurrentSoulEnergy.ToString();
    }

    // --- Slot Population ---

    private void Refresh()
    {
        if (_currentCatalog == null || _slotContainer == null || _slotPrefab == null) return;

        foreach (Transform child in _slotContainer)
            Destroy(child.gameObject);

        int currentSE = SoulEnergyManager.Instance != null ? SoulEnergyManager.Instance.CurrentSoulEnergy : 0;

        foreach (ShopItemEntry entry in _currentCatalog.items)
        {
            if (entry == null || entry.item == null) continue;

            GameObject slotObj = Instantiate(_slotPrefab, _slotContainer);

            Image icon = slotObj.GetComponent<Image>();
            if (icon != null)
            {
                icon.enabled = true;
                icon.sprite = entry.item.icon;
                icon.color = currentSE >= entry.price ? Color.white : new Color(1f, 1f, 1f, 0.4f);
            }

            TMP_Text priceText = slotObj.GetComponentInChildren<TMP_Text>(true);
            if (priceText != null)
            {
                priceText.gameObject.SetActive(true);
                priceText.text = entry.price.ToString();
            }

            ShopSlot shopSlot = slotObj.GetComponent<ShopSlot>();
            if (shopSlot == null)
                shopSlot = slotObj.AddComponent<ShopSlot>();

            shopSlot.Setup(entry, this);
        }
    }

    // --- Description Box & Typewriter ---

    public void ShowItemDescription(ShopItemEntry entry)
    {
        if (_descriptionText == null || entry == null || entry.item == null) return;

        ItemData item = entry.item;

        string localizedName = string.IsNullOrEmpty(item.displayName)
            ? item.itemName
            : LocalizationManager.Instance.GetTranslate(item.displayName);

        string localizedDesc = string.IsNullOrEmpty(item.itemDescription)
            ? string.Empty
            : LocalizationManager.Instance.GetTranslate(item.itemDescription);

        string fullText = !string.IsNullOrEmpty(localizedDesc)
            ? $"{localizedName} ({entry.price}): {localizedDesc}"
            : $"{localizedName} ({entry.price})";

        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        _typewriterCoroutine = StartCoroutine(TypewriterRoutine(fullText));
    }

    public void ClearItemDescription()
    {
        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        if (_descriptionText != null)
        {
            _descriptionText.text = string.Empty;
            _descriptionText.maxVisibleCharacters = 0;
        }
    }

    private IEnumerator TypewriterRoutine(string text)
    {
        _descriptionText.text = text;
        _descriptionText.ForceMeshUpdate();

        int totalCharacters = _descriptionText.textInfo.characterCount;
        _descriptionText.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalCharacters; i++)
        {
            _descriptionText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(_typewriterCharDelay);
        }
    }

}
