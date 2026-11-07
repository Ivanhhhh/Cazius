using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public enum InventoryTab
    {
        Regular,
        KeyItems,
        LoreItems
    }

    [Header("References")]
    public GameObject panel;
    public Transform slotContainer;   // The GridLayoutGroup parent
    public GameObject slotPrefab;
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private bool _savePanelOpenClose = true;
    [SerializeField] private Material _alwaysOnTopMaterial;

    [Header("Description Box & Typewriter")]
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private float _typewriterCharDelay = 0.02f;

    [Header("Tab UI Buttons")]
    [SerializeField] private Button _regularTabButton;
    [SerializeField] private Button _keyItemsTabButton;
    [SerializeField] private Button _loreItemsTabButton;

    [Header("Tab Button Colors")]
    [SerializeField] private Color _selectedTabColor = new Color32(0xB1, 0xFF, 0xFB, 0xFF);   // #B1FFFB
    [SerializeField] private Color _unselectedTabColor = new Color32(0x5A, 0x81, 0x7F, 0xFF); // #5A817F

    [Header("Tab State")]
    [SerializeField] private InventoryTab _currentTab = InventoryTab.Regular;

    private Image _regularTabImage;
    private Image _keyItemsTabImage;
    private Image _loreItemsTabImage;

    private Coroutine _typewriterCoroutine;

    private void Awake()
    {
        if (_regularTabButton != null)
        {
            _regularTabImage = _regularTabButton.GetComponent<Image>();
            _regularTabButton.onClick.AddListener(SelectRegularTab);
        }

        if (_keyItemsTabButton != null)
        {
            _keyItemsTabImage = _keyItemsTabButton.GetComponent<Image>();
            _keyItemsTabButton.onClick.AddListener(SelectKeyItemsTab);
        }
        if (_loreItemsTabButton != null)
        {
            _loreItemsTabImage = _loreItemsTabButton.GetComponent<Image>();
            _loreItemsTabButton.onClick.AddListener(SelectLoreItemsTab);
        }
    }

    private void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.onInventoryChanged.AddListener(Refresh);

        InventoryInputHandler.OnInventoryToggled += OnToggled;
        GameManager.Instance.inventoryUI = this;

        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.onInventoryChanged.RemoveListener(Refresh);

        InventoryInputHandler.OnInventoryToggled -= OnToggled;
    }

    private void OnDestroy()
    {
        InventoryInputHandler.OnInventoryToggled -= OnToggled;

        if (_regularTabButton != null)
            _regularTabButton.onClick.RemoveListener(SelectRegularTab);

        if (_keyItemsTabButton != null)
            _keyItemsTabButton.onClick.RemoveListener(SelectKeyItemsTab);
        if (_loreItemsTabButton != null)
            _loreItemsTabButton.onClick.RemoveListener(SelectLoreItemsTab);
    }

    public void SelectRegularTab()
    {
        _currentTab = InventoryTab.Regular;
        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    public void SelectKeyItemsTab()
    {
        _currentTab = InventoryTab.KeyItems;
        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    public void SelectLoreItemsTab()
    {
        _currentTab = InventoryTab.LoreItems;
        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    private void UpdateTabVisuals()
    {
        if (_regularTabImage != null)
        {
            _regularTabImage.color = (_currentTab == InventoryTab.Regular)
                ? _selectedTabColor
                : _unselectedTabColor;
        }

        if (_keyItemsTabImage != null)
        {
            _keyItemsTabImage.color = (_currentTab == InventoryTab.KeyItems)
                ? _selectedTabColor
                : _unselectedTabColor;
        }

        if (_loreItemsTabImage != null)
        {
            _loreItemsTabImage.color = (_currentTab == InventoryTab.LoreItems)
                ? _selectedTabColor
                : _unselectedTabColor;
        }
    }

    public void ShowItemDescription(ItemData item)
    {
        if (_descriptionText == null || item == null) return;

        string localizedName = string.IsNullOrEmpty(item.displayName)
            ? item.itemName
            : LocalizationManager.Instance.GetTranslate(item.displayName);

        string localizedDesc = string.IsNullOrEmpty(item.itemDescription)
            ? string.Empty
            : LocalizationManager.Instance.GetTranslate(item.itemDescription);

        string fullText = !string.IsNullOrEmpty(localizedDesc)
            ? $"{localizedName}: {localizedDesc}"
            : localizedName;

        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        _typewriterCoroutine = StartCoroutine(TypewriterRoutine(fullText));
    }

    public string ReturnItemDescription(ItemData item)
    {
        if (_descriptionText == null || item == null) return string.Empty;

        string localizedName = string.IsNullOrEmpty(item.displayName)
            ? item.itemName
            : LocalizationManager.Instance.GetTranslate(item.displayName);

        string localizedDesc = string.IsNullOrEmpty(item.itemDescription)
            ? string.Empty
            : LocalizationManager.Instance.GetTranslate(item.itemDescription);

        string fullText = !string.IsNullOrEmpty(localizedDesc)
            ? $"{localizedName}: {localizedDesc}"
            : localizedName;

        return fullText;
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
        if (_descriptionText == null) yield break;

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

    private void OnToggled(bool isOpen)
    {
        Debug.Log("4. El evento llegó a la UI. isOpen: " + isOpen);

        if (isOpen)
        {
            OpenInventory();
        }
        else
        {
            Debug.Log("5. Ejecutando CloseInventory()...");
            CloseInventory();
        }
    }

    public void OpenInventory()
    {
        panel.SetActive(true);
        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    public void CloseInventory()
    {
        panel.SetActive(false);
        ClearItemDescription();

        if (_questPanel != null && !_savePanelOpenClose)
            _questPanel.SetActive(false);
    }

    private void Refresh()
    {
        if (Inventory.Instance == null) return;

        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        System.Collections.Generic.List<ItemData> targetList;
        int maxSlots;

        switch (_currentTab)
        {
            case InventoryTab.KeyItems:
                targetList = Inventory.Instance.keyItems;
                maxSlots = Inventory.Instance.maxKeySlots;
                break;

            case InventoryTab.LoreItems:
                targetList = Inventory.Instance.loreItems;
                maxSlots = Inventory.Instance.maxLoreSlots;
                break;

            default:
                targetList = Inventory.Instance.regularItems;
                maxSlots = Inventory.Instance.maxRegularSlots;
                break;
        }

        for (int i = 0; i < maxSlots; i++)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            bool hasItem = i < targetList.Count;

            var icon = slot.GetComponent<Image>();
            icon.material = _alwaysOnTopMaterial;
            icon.enabled = hasItem;

            var amountText = slot.GetComponentInChildren<TextMeshProUGUI>(true);

            var slotHover = slot.GetComponent<InventorySlot>();
            if (slotHover == null)
                slotHover = slot.AddComponent<InventorySlot>();

            if (hasItem)
            {
                var item = targetList[i];
                icon.sprite = item.icon;

                slotHover.Setup(item, this);

                if (amountText != null)
                    if (amountText != null)
                    {
                        bool showQuantity =
                            item.maxStackSize > 1 ||
                            item.value > 1 ||
                            item.itemType == ItemType.Ammo ||
                            item.itemType == ItemType.Scrap;

                        if (showQuantity)
                        {
                            amountText.text = item.value.ToString();
                            amountText.gameObject.SetActive(true);
                        }
                        else
                        {
                            amountText.gameObject.SetActive(false);
                        }
                    }

                int index = i;
                slot.GetComponent<Button>().onClick.AddListener(() =>
                    Inventory.Instance.UseItem(targetList[index])
                );
            }
            else
            {
                slotHover.Setup(null, this);
                if (amountText != null) amountText.gameObject.SetActive(false);
            }
        }
    }
}