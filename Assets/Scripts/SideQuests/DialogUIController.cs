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

    private string[] _pages;
    private int _currentPage;
    private Action _onAccept;
    private Action _onClose;

    // Stored handler references for safe unsubscription
    private Action<InputAction.CallbackContext> _onAdvance;
    private Action<InputAction.CallbackContext> _onSkip;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        nextButton.onClick.AddListener(OnNextPressed);
        acceptButton.onClick.AddListener(OnAcceptPressed);
        closeButton.onClick.AddListener(OnClosePressed);

        typewriter.OnComplete += OnPageTypingComplete;

        // Assign handlers once so the same reference is used for both sub and unsub
        _onAdvance = _ => OnAdvancePressed();
        _onSkip = _ => OnSkipPressed();

        dialogPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameInputManager.Instance.Controls.Dialog.Advance.performed += _onAdvance;
        GameInputManager.Instance.Controls.Dialog.Skip.performed += _onSkip;
    }

    private void OnDisable()
    {
        GameInputManager.Instance.Controls.Dialog.Advance.performed -= _onAdvance;
        GameInputManager.Instance.Controls.Dialog.Skip.performed -= _onSkip;
    }

    // --- Public API called by QuestGiverNPC ---

    public void OpenDialog(string[] pages, Action onAccept = null, Action onClose = null)
    {
        _pages = pages;
        _currentPage = 0;
        _onAccept = onAccept;
        _onClose = onClose;

        SetActionButtons(false);
        nextButton.gameObject.SetActive(true);
        nextButton.interactable = false;

        dialogPanel.SetActive(true);
        GameInputManager.Instance.EnableDialogInput();
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
            SetActionButtons(true);
        }
        else
        {
            nextButton.interactable = true;
        }
    }

    // --- Input handlers ---

    // Advance (A key) — skip typing if in progress, or advance to next page if done
    private void OnAdvancePressed()
    {
        OnNextPressed();
    }

    // Skip (X key) — only completes current page typing instantly, never advances
    private void OnSkipPressed()
    {
        if (!typewriter.IsComplete)
            typewriter.Skip();
    }

    private void OnNextPressed()
    {
        if (!typewriter.IsComplete)
        {
            typewriter.Skip();
            return;
        }

        // On last page, do nothing — player must use Accept/Close buttons
        if (_currentPage >= _pages.Length - 1) return;

        _currentPage++;
        nextButton.interactable = false;
        ShowCurrentPage();
    }

    // --- Action buttons ---

    private void SetActionButtons(bool active)
    {
        bool hasAccept = _onAccept != null;
        acceptButton.gameObject.SetActive(active && hasAccept);
        closeButton.gameObject.SetActive(active);
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
    }
}
