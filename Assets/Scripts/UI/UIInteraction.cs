using UnityEngine;
using TMPro;

public class UIInteraction : MonoBehaviour
{
    [SerializeField] private GameObject _containerUI;
    [SerializeField] private PlayerInteract _playerInteract;
    [SerializeField] private TextMeshProUGUI _interactText;

    private void Update()
    {
        if (_playerInteract.GetInteractableObject() != null)
        {
            Show(_playerInteract.GetInteractableObject());
        }
        else
        {
            Hide();
        }
    }

    private void Show(IInteractable interactable)
    {
        _containerUI.SetActive(true);
        _interactText.text = interactable.GetInteractText();
    }

    public void Hide()
    {
        _containerUI.SetActive(false);
    }
}
