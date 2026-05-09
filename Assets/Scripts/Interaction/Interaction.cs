using UnityEngine;

public class Interaction : MonoBehaviour
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

        IInteractable interactable = _raycast.CurrentTarget.GetComponent<IInteractable>();

        if (interactable == null) return;

        interactable.Interact(_inventory);
    }
}




