using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogUIController : MonoBehaviour
{
    public static DialogUIController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private DialogTypewriter typewriter;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button declineButton;

    public static event Action OnDialogOpened;
    public static event Action OnDialogClosed;

    private string[] _pages;
    private int _currentPage;
    private Action _onAccept;
    private Action _onClose;

    private Action<InputAction.CallbackContext> _onAdvance;
    private Action<InputAction.CallbackContext> _onDecline;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        nextButton.onClick.AddListener(OnNextPressed);
        acceptButton.onClick.AddListener(OnAcceptPressed);
        closeButton.onClick.AddListener(OnClosePressed);
        declineButton.onClick.AddListener(OnDeclinePressed);

        typewriter.OnComplete += OnPageTypingComplete;

        _onAdvance = _ => OnAdvancePressed();
        _onDecline = _ => OnDeclinePressed();

        dialogPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameInputManager.Instance.Controls.Dialog.NextAdvanceAcceptClose.performed += _onAdvance;
        GameInputManager.Instance.Controls.Dialog.Decline.performed += _onDecline;
    }

    private void OnDisable()
    {
        GameInputManager.Instance.Controls.Dialog.NextAdvanceAcceptClose.performed -= _onAdvance;
        GameInputManager.Instance.Controls.Dialog.Decline.performed -= _onDecline;
    }

    // --- Public API ---

    public void OpenDialog(string[] pages, Action onAccept = null, Action onClose = null)
    {
        _pages = pages;
        _currentPage = 0;
        _onAccept = onAccept;
        _onClose = onClose;

        HideAllActionButtons();
        nextButton.gameObject.SetActive(true);
        nextButton.interactable = false;

        dialogPanel.SetActive(true);
        GameInputManager.Instance.EnableDialogInput();
        OnDialogOpened?.Invoke();
        ShowCurrentPage();
    }

    // --- Page Logic ---

    private void ShowCurrentPage()
    {
        typewriter.Play(_pages[_currentPage]);
    }

    private void OnPageTypingComplete()
    {
        bool isLastPage = _currentPage >= _pages.Length - 1;

        if (isLastPage)
        {
            nextButton.gameObject.SetActive(false);
            ShowActionButtons();
        }
        else
        {
            nextButton.interactable = true;
        }
    }

    // --- Input Handlers ---

    // A key > skip typing > advance page > Accept or Close on last page
    private void OnAdvancePressed()
    {
        if (acceptButton.gameObject.activeSelf)
        {
            OnAcceptPressed();
            return;
        }

        if (closeButton.gameObject.activeSelf)
        {
            OnClosePressed();
            return;
        }

        OnNextPressed();
    }

    // D key > Decline on offer, ignored otherwise
    private void OnDeclinePressed()
    {
        if (declineButton.gameObject.activeSelf)
            CloseDialogSequence(null);
    }

    private void OnNextPressed()
    {
        if (!typewriter.IsComplete)
        {
            typewriter.Skip();
            return;
        }

        if (_currentPage >= _pages.Length - 1) return;

        _currentPage++;
        nextButton.interactable = false;
        ShowCurrentPage();
    }

    // --- Action Button Handlers ---

    private void ShowActionButtons()
    {
        bool isOffer = _onAccept != null;
        acceptButton.gameObject.SetActive(isOffer);
        declineButton.gameObject.SetActive(isOffer);
        closeButton.gameObject.SetActive(!isOffer);
    }

    private void HideAllActionButtons()
    {
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    private void OnAcceptPressed()
    {
        CloseDialogSequence(_onAccept);
    }

    private void OnClosePressed()
    {
        CloseDialogSequence(_onClose);
    }

    private void CloseDialogSequence(Action callback)
    {
        // 1. Visually hide current dialogue
        dialogPanel.SetActive(false);

        // 2. Execute callback (e.g., remove item, advance stage, or open next dialogue)
        callback?.Invoke();

        // 3. Only disable dialogue input & free player if NO new dialogue was opened by the callback
        if (!dialogPanel.activeSelf)
        {
            GameInputManager.Instance.DisableDialogInput();
            OnDialogClosed?.Invoke();
        }
    }
}