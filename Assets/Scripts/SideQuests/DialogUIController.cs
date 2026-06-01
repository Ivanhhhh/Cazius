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

    // Static events so UIInteraction can subscribe without a direct reference
    public static event Action OnDialogOpened;
    public static event Action OnDialogClosed;

    private string[] _pages;
    private int _currentPage;
    private Action _onAccept;
    private Action _onClose;

    // Stored handler references for safe unsubscription
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

    // --- Public API called by QuestGiverNPC ---

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

    // --- Page logic ---

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

    // --- Input handlers ---

    // A key — skip typing > advance page > Accept or Close on last page
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

    // D key — Decline on offer, ignored otherwise
    private void OnDeclinePressed()
    {
        if (declineButton.gameObject.activeSelf)
            CloseDialog();
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

    // --- Action buttons ---

    // Offer state > Accept (A) + Decline (D)
    // Active/Completed state > Close (A) only
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
        _onAccept?.Invoke();
        CloseDialog();
    }

    private void OnClosePressed()
    {
        _onClose?.Invoke();
        CloseDialog();
    }

    private void CloseDialog()
    {
        dialogPanel.SetActive(false);
        GameInputManager.Instance.DisableDialogInput();
        OnDialogClosed?.Invoke();
    }
}
