using UnityEngine;
using TMPro;

public class UIInteraction : MonoBehaviour
{
    [SerializeField] private GameObject _containerUI;
    [SerializeField] private TextMeshProUGUI _interactText;

    private bool _dialogOpen = false;

    private void OnEnable()
    {
        PlayerInteract.OnInteractableChanged += OnInteractableChanged;
        DialogUIController.OnDialogOpened += OnDialogOpened;
        DialogUIController.OnDialogClosed += OnDialogClosed;
    }

    private void OnDisable()
    {
        PlayerInteract.OnInteractableChanged -= OnInteractableChanged;
        DialogUIController.OnDialogOpened -= OnDialogOpened;
        DialogUIController.OnDialogClosed -= OnDialogClosed;
    }

    private void OnInteractableChanged(IEInteractable interactable)
    {
        if (_dialogOpen) return;

        if (interactable != null)
            Show(interactable);
        else
            Hide();
    }

    private void Show(IEInteractable interactable)
    {
        _containerUI.SetActive(true);
        _interactText.text = interactable.GetInteractText();
    }

    private void Hide()
    {
        _containerUI.SetActive(false);
    }

    private void OnDialogOpened()
    {
        _dialogOpen = true;
        Hide();
    }

    private void OnDialogClosed()
    {
        _dialogOpen = false;
        // Re-evaluate — if player is still in front of an interactable, show prompt again
        // PlayerInteract will fire OnInteractableChanged on next frame naturally
    }
}