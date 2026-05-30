using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private DialogTypewriter typewriter;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button closeButton;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    private string[] _pages;
    private int _currentPage;
    private Action _onAccept;
    private Action _onClose;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnNextPressed);
        acceptButton.onClick.AddListener(OnAcceptPressed);
        closeButton.onClick.AddListener(OnClosePressed);

        typewriter.OnComplete += OnPageTypingComplete;

        dialogPanel.SetActive(false);
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

        dialogPanel.SetActive(true);
        SwitchToDialogInput();
        ShowCurrentPage();
    }

    // --- Page logic ---

    private void ShowCurrentPage()
    {
        nextButton.interactable = false;
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

    private void OnNextPressed()
    {
        if (!typewriter.IsComplete)
        {
            typewriter.Skip();
            return;
        }

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
        SwitchToPlayerInput();
    }

    // --- Input map switching ---

    private void SwitchToDialogInput()
    {
        playerInput.SwitchCurrentActionMap("Dialog");
    }

    private void SwitchToPlayerInput()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    // --- Keyboard / gamepad confirm binding (Dialog action map) ---

    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        OnNextPressed();
    }

    public void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        // Only close if on last page and typing is done
        if (!nextButton.gameObject.activeSelf && closeButton.gameObject.activeSelf)
            OnClosePressed();
    }
}