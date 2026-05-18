using UnityEngine;

public class Interaction : MonoBehaviour // Lo tiene el player con tag "Player" y layer Player
{
    [SerializeField] private InteractionRaycast _raycast;
    [SerializeField] private KeyInventorySystem _inventory;

    public PlayerControls _controls;

    void Start()
    {
        _controls = GameInputManager.Instance.Controls;
    }

    private void Update()
    {
        HandleInteraction();
        HandleConsume();
    }

    private void HandleInteraction()
    {
        Debug.Log("#### HandleInteraction");

        bool canInteract = _controls.Player.Interaction.WasPressedThisFrame() && _raycast.CurrentTarget != null;

        if (!canInteract) return;

        IInteractable interactable = _raycast.CurrentTarget.GetComponentInParent<IInteractable>();

        if (interactable == null) return;

        interactable.Interact(_inventory);
    }

    private void HandleConsume()
    {
        if (_controls.Player.Consume.WasPressedThisFrame() && _inventory.CurrentSoulEnergy > 0)
        {
            _inventory.RemoveSoulEnergy();

            if (SoulUIManager.Instance != null)
            {
                SoulUIManager.Instance.UpdateUI(_inventory.CurrentSoulEnergy);
            }

            Debug.Log("Soul Energy consumed");
        }
    }
}




