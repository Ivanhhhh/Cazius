using UnityEngine;

public class Interaction : MonoBehaviour // Lo tiene el player con tag "Player" y layer Player
{
    [SerializeField] private InteractionRaycast _raycast;
    [SerializeField] private InventorySystem _inventory;

    private void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        bool canInteract = Input.GetKeyDown(KeyCode.F) && _raycast.CurrentTarget != null;

        if (!canInteract) return;

        IInteractable interactable = _raycast.CurrentTarget.GetComponentInParent<IInteractable>();

        if (interactable == null) return;

        interactable.Interact(_inventory);
    }
}




