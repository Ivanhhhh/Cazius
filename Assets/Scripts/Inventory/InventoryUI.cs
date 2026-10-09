using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public enum InventoryTab
    {
        Regular,
        KeyItems
    }

    [Header("References")]
    public GameObject panel;
    public Transform slotContainer;   // The GridLayoutGroup parent
    public GameObject slotPrefab;
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private bool _savePanelOpenClose = true;
    [SerializeField] private Material _alwaysOnTopMaterial;

    [Header("Tab UI Buttons")]
    [SerializeField] private Button _regularTabButton;
    [SerializeField] private Button _keyItemsTabButton;

    [Header("Tab Button Colors")]
    [SerializeField] private Color _selectedTabColor = new Color32(0xB1, 0xFF, 0xFB, 0xFF);   // #B1FFFB
    [SerializeField] private Color _unselectedTabColor = new Color32(0x5A, 0x81, 0x7F, 0xFF); // #5A817F

    [Header("Tab State")]
    [SerializeField] private InventoryTab _currentTab = InventoryTab.Regular;

    private Image _regularTabImage;
    private Image _keyItemsTabImage;

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
    }

    private void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.onInventoryChanged.AddListener(Refresh);

        InventoryInputHandler.OnInventoryToggled += OnToggled;

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
    }

    public void SelectRegularTab()
    {
        _currentTab = InventoryTab.Regular;
        UpdateTabVisuals();
        Refresh();
    }

    public void SelectKeyItemsTab()
    {
        _currentTab = InventoryTab.KeyItems;
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
        UpdateTabVisuals();
        Refresh();
    }

    public void CloseInventory()
    {
        panel.SetActive(false);

        if (_questPanel != null && !_savePanelOpenClose)
            _questPanel.SetActive(false);
    }

    private void Refresh()
    {
        if (Inventory.Instance == null) return;

        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        var targetList = (_currentTab == InventoryTab.Regular)
            ? Inventory.Instance.regularItems
            : Inventory.Instance.keyItems;

        int maxSlots = (_currentTab == InventoryTab.Regular)
            ? Inventory.Instance.maxRegularSlots
            : Inventory.Instance.maxKeySlots;

        for (int i = 0; i < maxSlots; i++)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            bool hasItem = i < targetList.Count;

            var icon = slot.GetComponent<Image>();
            icon.material = _alwaysOnTopMaterial;
            icon.enabled = hasItem;

            var amountText = slot.GetComponentInChildren<TextMeshProUGUI>(true);

            if (hasItem)
            {
                var item = targetList[i];
                icon.sprite = item.icon;

                if (amountText != null)
                {
                    if (item.itemType == ItemType.Ammo || item.itemType == ItemType.Scrap)
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
                if (amountText != null) amountText.gameObject.SetActive(false);
            }
        }
    }

}
