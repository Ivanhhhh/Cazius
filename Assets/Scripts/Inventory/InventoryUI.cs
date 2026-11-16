using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public enum InventoryTab { Regular, KeyItems, LoreItems }

    public static InventoryUI Instance { get; private set; }

    [Header("References")]
    public GameObject panel;
    public Transform slotContainer;
    public GameObject slotPrefab;
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private bool _savePanelOpenClose = true;
    [SerializeField] private Material _alwaysOnTopMaterial;

    [Header("Description Box & Typewriter")]
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private float _typewriterCharDelay = 0.02f;

    [Header("Lore Note Preview")]
    [SerializeField] private GameObject _loreNotePanel;
    [SerializeField] private TMP_Text _loreNoteText;

    [Header("Tab UI Buttons")]
    [SerializeField] private Button _regularTabButton;
    [SerializeField] private Button _keyItemsTabButton;
    [SerializeField] private Button _loreItemsTabButton;

    [Header("Tab Button Colors")]
    [SerializeField] private Color _selectedTabColor = new Color32(0xB1, 0xFF, 0xFB, 0xFF);
    [SerializeField] private Color _unselectedTabColor = new Color32(0x5A, 0x81, 0x7F, 0xFF);

    [Header("Tab State")]
    [SerializeField] private InventoryTab _currentTab = InventoryTab.Regular;

    [Header("Slot Pulse")]
    [SerializeField] private bool _createSlotPulses = true;
    [SerializeField] private Transform _pulseContainer;
    [SerializeField] private UIInventorySlotAnimations _pulsePrefab;

    [Header("Debug")]
    [SerializeField] private bool _debugPulseAlignment = true;

    // Listas de reciclaje (persisten toda la escena)
    private readonly List<GameObject> _spawnedSlots = new();
    private readonly List<UIInventorySlotAnimations> _slotPulses = new();
    private readonly List<RectTransform> _slotRects = new();
    private readonly HashSet<string> _disabledPulseItemIDs = new();

    private Image _regularTabImage;
    private Image _keyItemsTabImage;
    private Image _loreItemsTabImage;
    private Coroutine _typewriterCoroutine;
    private Coroutine _loreTypewriterCoroutine;

    private bool _isMoveMode;
    private int _moveSourceIndex = -1;
    public bool IsMoveMode => _isMoveMode;


    private void Awake()
    {
        Instance = this;
        if (_regularTabButton != null) { _regularTabImage = _regularTabButton.GetComponent<Image>(); _regularTabButton.onClick.AddListener(SelectRegularTab); }
        if (_keyItemsTabButton != null) { _keyItemsTabImage = _keyItemsTabButton.GetComponent<Image>(); _keyItemsTabButton.onClick.AddListener(SelectKeyItemsTab); }
        if (_loreItemsTabButton != null) { _loreItemsTabImage = _loreItemsTabButton.GetComponent<Image>(); _loreItemsTabButton.onClick.AddListener(SelectLoreItemsTab); }
    }

    private void OnEnable()
    {
        if (Inventory.Instance != null) Inventory.Instance.onInventoryChanged.AddListener(Refresh);
        InventoryInputHandler.OnInventoryToggled += OnToggled;
        GameManager.Instance.inventoryUI = this;

        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null) Inventory.Instance.onInventoryChanged.RemoveListener(Refresh);
        InventoryInputHandler.OnInventoryToggled -= OnToggled;

    }

    private void OnDestroy()
    {
        InventoryInputHandler.OnInventoryToggled -= OnToggled;
        if (_regularTabButton != null) _regularTabButton.onClick.RemoveListener(SelectRegularTab);
        if (_keyItemsTabButton != null) _keyItemsTabButton.onClick.RemoveListener(SelectKeyItemsTab);
        if (_loreItemsTabButton != null) _loreItemsTabButton.onClick.RemoveListener(SelectLoreItemsTab);
    }

    // Tabs (al cambiar de pestaña, permitimos que se vuelva a sincronizar)
    public void SelectRegularTab()
    {
        UIInventoryTooltip.Instance?.Hide();
        CancelMoveMode();
        StopAllPulses();
        _currentTab = InventoryTab.Regular;
        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    public void SelectKeyItemsTab()
    {
        UIInventoryTooltip.Instance?.Hide();
        CancelMoveMode();
        StopAllPulses();
        _currentTab = InventoryTab.KeyItems;
        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    public void SelectLoreItemsTab()
    {
        UIInventoryTooltip.Instance?.Hide();
        CancelMoveMode();
        StopAllPulses();
        _currentTab = InventoryTab.LoreItems;
        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    private void UpdateTabVisuals()
    {
        if (_regularTabImage != null) _regularTabImage.color = (_currentTab == InventoryTab.Regular) ? _selectedTabColor : _unselectedTabColor;
        if (_keyItemsTabImage != null) _keyItemsTabImage.color = (_currentTab == InventoryTab.KeyItems) ? _selectedTabColor : _unselectedTabColor;
        if (_loreItemsTabImage != null) _loreItemsTabImage.color = (_currentTab == InventoryTab.LoreItems) ? _selectedTabColor : _unselectedTabColor;
    }

    // Descripción

    public void ShowItemDescription(ItemData item)
    {
        if (item == null) return;
        if (item.isLoreItem && !string.IsNullOrEmpty(item.loreText) && _loreNotePanel != null)
        {
            _loreNotePanel.SetActive(true);
            if (_loreNoteText != null)
            {
                string translatedLore = LocalizationManager.Instance.GetTranslate(item.loreText).Replace("\\n", "\n");
                if (_loreTypewriterCoroutine != null) StopCoroutine(_loreTypewriterCoroutine);
                _loreTypewriterCoroutine = StartCoroutine(LoreTypewriterRoutine(translatedLore));
            }
        }
        else HideLoreNote();

        if (_descriptionText == null) return;
        string localizedName = string.IsNullOrEmpty(item.displayName) ? item.itemName : LocalizationManager.Instance.GetTranslate(item.displayName);
        string localizedDesc = string.IsNullOrEmpty(item.itemDescription) ? string.Empty : LocalizationManager.Instance.GetTranslate(item.itemDescription);
        string fullText = !string.IsNullOrEmpty(localizedDesc) ? $"{localizedName}: {localizedDesc}" : localizedName;

        if (_typewriterCoroutine != null) StopCoroutine(_typewriterCoroutine);
        _typewriterCoroutine = StartCoroutine(TypewriterRoutine(fullText));
    }

    public string ReturnItemDescription(ItemData item)
    {
        if (_descriptionText == null || item == null) return string.Empty;
        string localizedName = string.IsNullOrEmpty(item.displayName) ? item.itemName : LocalizationManager.Instance.GetTranslate(item.displayName);
        string localizedDesc = string.IsNullOrEmpty(item.itemDescription) ? string.Empty : LocalizationManager.Instance.GetTranslate(item.itemDescription);
        return !string.IsNullOrEmpty(localizedDesc) ? $"{localizedName}: {localizedDesc}" : localizedName;
    }

    public void ClearItemDescription()
    {
        if (_typewriterCoroutine != null) StopCoroutine(_typewriterCoroutine);
        if (_descriptionText != null) { _descriptionText.text = string.Empty; _descriptionText.maxVisibleCharacters = 0; }
        HideLoreNote();
    }

    private void HideLoreNote()
    {
        if (_loreTypewriterCoroutine != null) { StopCoroutine(_loreTypewriterCoroutine); _loreTypewriterCoroutine = null; }
        if (_loreNotePanel != null) _loreNotePanel.SetActive(false);
        if (_loreNoteText != null) { _loreNoteText.text = string.Empty; _loreNoteText.maxVisibleCharacters = 0; }
    }

    private IEnumerator TypewriterRoutine(string text)
    {
        if (_descriptionText == null) yield break;
        _descriptionText.text = text;
        _descriptionText.ForceMeshUpdate();
        int total = _descriptionText.textInfo.characterCount;
        _descriptionText.maxVisibleCharacters = 0;
        for (int i = 0; i <= total; i++) { _descriptionText.maxVisibleCharacters = i; yield return new WaitForSecondsRealtime(_typewriterCharDelay); }
    }

    private IEnumerator LoreTypewriterRoutine(string text)
    {
        if (_loreNoteText == null) yield break;
        _loreNoteText.text = text;
        _loreNoteText.ForceMeshUpdate();
        int total = _loreNoteText.textInfo.characterCount;
        _loreNoteText.maxVisibleCharacters = 0;
        for (int i = 0; i <= total; i++) { _loreNoteText.maxVisibleCharacters = i; yield return new WaitForSecondsRealtime(_typewriterCharDelay); }
        _loreTypewriterCoroutine = null;
    }

    // Open / Close

    private void OnToggled(bool isOpen) { if (isOpen) OpenInventory(); else CloseInventory(); }

    public void OpenInventory()
    {
        panel.SetActive(true);
        ClearItemDescription();
        UpdateTabVisuals();
        Refresh();
    }

    public void CloseInventory()
    {
        CancelMoveMode();
        UIInventoryTooltip.Instance?.HideImmediate();
        panel.SetActive(false);
        ClearItemDescription();
        if (_questPanel != null && !_savePanelOpenClose) _questPanel.SetActive(false);
    }

    // Move mode

    public void BeginMoveModeByItem(ItemData item)
    {
        if (item == null) return;
        var list = GetCurrentList();
        if (list == null) return;
        int idx = list.IndexOf(item);
        if (idx < 0) return;
        BeginMoveMode(idx);
    }

    public void BeginMoveMode(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slotPulses.Count) return;
        var item = GetItemAtSlot(slotIndex);
        if (item == null) return;

        CancelMoveMode();
        _isMoveMode = true;
        _moveSourceIndex = slotIndex;

        var pulse = GetSlotPulse(slotIndex);
        if (pulse != null) pulse.StartPulse();
    }

    public void CancelMoveMode()
    {
        _isMoveMode = false;
        _moveSourceIndex = -1;
        StopAllPulses();
    }

    public void SetSlotHover(int slotIndex, bool hovered)
    {
        if (!_isMoveMode) return;
        if (slotIndex < 0 || slotIndex >= _slotPulses.Count) return;
        var pulse = _slotPulses[slotIndex];
        if (pulse == null) return;
        if (hovered) pulse.StartPulse();
        else if (slotIndex != _moveSourceIndex) pulse.StopPulse(true);
    }

    public void HandleSlotClicked(int targetIndex)
    {
        if (!_isMoveMode) return;
        if (targetIndex == _moveSourceIndex) { CancelMoveMode(); return; }

        var list = GetCurrentList();
        if (list == null) return;

        while (list.Count <= Mathf.Max(_moveSourceIndex, targetIndex)) list.Add(null);

        var target = list[targetIndex];
        if (target != null)
            Inventory.SwapSlots(list, _moveSourceIndex, targetIndex);
        else
            Inventory.MoveSlot(list, _moveSourceIndex, targetIndex);

        Inventory.TrimTrailingNulls(list);

        // Esto apaga _isMoveMode y llama a StopAllPulses()
        CancelMoveMode(); 
        
        Inventory.Instance.onInventoryChanged?.Invoke();
    }

    // Refresh (Lógica sin destrucción)

    private void Refresh()
    {
        if (Inventory.Instance == null) return;
        if (_isMoveMode) CancelMoveMode();

        List<ItemData> targetList;
        int maxSlots;
        switch (_currentTab)
        {
            case InventoryTab.KeyItems: targetList = Inventory.Instance.keyItems; maxSlots = Inventory.Instance.maxKeySlots; break;
            case InventoryTab.LoreItems: targetList = Inventory.Instance.loreItems; maxSlots = Inventory.Instance.maxLoreSlots; break;
            default: targetList = Inventory.Instance.regularItems; maxSlots = Inventory.Instance.maxRegularSlots; break;
        }

        // 1. Crear SOLO si faltan slots (Nunca se destruyen)
        while (_spawnedSlots.Count < maxSlots)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            var slotRect = slot.GetComponent<RectTransform>();

            _spawnedSlots.Add(slot);
            _slotRects.Add(slotRect);

            UIInventorySlotAnimations pulse = null;
            if (_createSlotPulses && slotRect != null)
            {
                pulse = CreateSlotPulse(); // Ya no necesita el slotRect
            }
            _slotPulses.Add(pulse);
        }

        // 2. Actualizar datos de los slots existentes
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            if (i >= maxSlots)
            {
                _spawnedSlots[i].SetActive(false);
                if (_slotPulses[i] != null) _slotPulses[i].gameObject.SetActive(false);
                continue;
            }

            _spawnedSlots[i].SetActive(true);
            if (_slotPulses[i] != null) _slotPulses[i].gameObject.SetActive(true);

            bool hasItem = i < targetList.Count && targetList[i] != null;

            var icon = _spawnedSlots[i].GetComponent<Image>();
            icon.material = _alwaysOnTopMaterial;

            // IMPORTANTE: Mantenemos la imagen activa para que el slot reciba los clics (y funcionen los pulsos)
            icon.enabled = true;
            icon.raycastTarget = true;

            var amountText = _spawnedSlots[i].GetComponentInChildren<TextMeshProUGUI>(true);
            var slotHover = _spawnedSlots[i].GetComponent<InventorySlot>();
            if (slotHover == null) slotHover = _spawnedSlots[i].AddComponent<InventorySlot>();

            if (hasItem)
            {
                var item = targetList[i];
                icon.sprite = item.icon;

                // Si hay sprite, aseguramos Alpha 1 (sin tocar el color del prefab)
                Color c = icon.color;
                c.a = 1f;
                icon.color = c;

                slotHover.Setup(item, this, i);

                if (amountText != null)
                {
                    bool showQuantity = item.maxStackSize > 1 || item.value > 1 || item.itemType == ItemType.Ammo || item.itemType == ItemType.Scrap;
                    if (showQuantity) { amountText.text = item.value.ToString(); amountText.gameObject.SetActive(true); }
                    else amountText.gameObject.SetActive(false);
                }

                if (_slotPulses[i] != null)
                {
                    string key = GetPulseKey(item);
                    if (!string.IsNullOrEmpty(key) && _disabledPulseItemIDs.Contains(key))
                        _slotPulses[i].StopPulse(resetToVisible: true);
                }
            }
            else
            {
                icon.sprite = null; // Sin sprite, el cuadro blanco desaparece

                // Si no hay sprite, ponemos Alpha 0 (pero sigue siendo clickeable)
                Color c = icon.color;
                c.a = 0f;
                icon.color = c;

                slotHover.Setup(null, this, i);
                if (amountText != null) amountText.gameObject.SetActive(false);
                if (_slotPulses[i] != null) _slotPulses[i].StopPulse(resetToVisible: true);
            }
        }

        // ¡Y listo! No hace falta sincronizar posiciones manualmente. 
        // El GridLayoutGroup de _pulseContainer se encarga de moverlos.
    }

    // Pulse creation / sync


    private UIInventorySlotAnimations CreateSlotPulse()
    {
        if (_pulsePrefab == null) return null;

        Transform parent = _pulseContainer != null ? _pulseContainer : slotContainer;
        UIInventorySlotAnimations pulse = Instantiate(_pulsePrefab, parent);

        // FORZAR RAYCAST TARGET A FALSE PARA QUE NO BLOQUEE EL MOUSE
        var img = pulse.GetComponent<Image>();
        if (img != null) img.raycastTarget = false;

        var canvasGroup = pulse.GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;

        pulse.gameObject.SetActive(false);
        return pulse;
    }

    // Helpers

    public UIInventorySlotAnimations GetSlotPulse(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slotPulses.Count) return null;
        return _slotPulses[slotIndex];
    }

    public int SlotPulseCount => _slotPulses.Count;

    public ItemData GetItemAtSlot(int slotIndex)
    {
        var list = GetCurrentList();
        if (list == null) return null;
        return (slotIndex >= 0 && slotIndex < list.Count) ? list[slotIndex] : null;
    }

    private List<ItemData> GetCurrentList()
    {
        if (Inventory.Instance == null) return null;
        switch (_currentTab)
        {
            case InventoryTab.KeyItems: return Inventory.Instance.keyItems;
            case InventoryTab.LoreItems: return Inventory.Instance.loreItems;
            default: return Inventory.Instance.regularItems;
        }
    }

    private static string GetPulseKey(ItemData item)
    {
        if (item == null) return null;
        if (!string.IsNullOrEmpty(item.itemID)) return "ID:" + item.itemID;
        if (!string.IsNullOrEmpty(item.name)) return "NAME:" + item.name;
        return null;
    }

    public void EnableSlotPulse(int slotIndex)
    {
        var pulse = GetSlotPulse(slotIndex);
        if (pulse == null) return;
        var item = GetItemAtSlot(slotIndex);
        string key = GetPulseKey(item);
        if (!string.IsNullOrEmpty(key)) _disabledPulseItemIDs.Remove(key);
        pulse.StartPulse();
    }

    public void DisableSlotPulse(int slotIndex)
    {
        var pulse = GetSlotPulse(slotIndex);
        if (pulse == null) return;
        var item = GetItemAtSlot(slotIndex);
        string key = GetPulseKey(item);
        if (!string.IsNullOrEmpty(key)) _disabledPulseItemIDs.Add(key);
        pulse.StopPulse(resetToVisible: true);
    }

    public void StopAllPulses()
    {
        for (int i = 0; i < _slotPulses.Count; i++)
        {
            if (_slotPulses[i] != null)
                _slotPulses[i].StopPulse(resetToVisible: true);
        }
    }
}