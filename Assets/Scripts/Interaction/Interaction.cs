using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour // Lo tiene el player con tag "Player" y layer Player
{
    [SerializeField] private InteractionRaycast _raycast;
    [SerializeField] private InventorySystem _inventory;

    [Header("Inputs")]
    [SerializeField] private InputActionAsset InputActions;
    private InputAction m_interactAction;
    private InputAction m_consumeAction;

    void Awake()
    {
        var playerMap = InputActions.FindActionMap("Player");
        m_interactAction = playerMap.FindAction("Interaction");
        m_consumeAction = playerMap.FindAction("Consume");
    }

    void OnEnable() { InputActions.Enable(); }
    void OnDisable() { InputActions.Disable(); }

    private void Update()
    {
        HandleInteraction();
        HandleConsume();
    }

    private void HandleInteraction()
    {
        bool canInteract = m_interactAction.WasPressedThisFrame() && _raycast.CurrentTarget != null;

        if (!canInteract) return;

        IInteractable interactable = _raycast.CurrentTarget.GetComponentInParent<IInteractable>();

        if (interactable == null) return;

        interactable.Interact(_inventory);
    }

    private void HandleConsume()
    {
        if (m_consumeAction.WasPressedThisFrame() && _inventory.CurrentSoulEnergy > 0)
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




