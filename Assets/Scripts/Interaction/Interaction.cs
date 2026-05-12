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

    void Awake()
    {
        m_interactAction = InputActions.FindActionMap("Player").FindAction("Interaction");
    }

    void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        Debug.Log("#### HandleInteraction");

        bool canInteract = m_interactAction.WasPressedThisFrame() && _raycast.CurrentTarget != null;

        if (!canInteract) return;

        IInteractable interactable = _raycast.CurrentTarget.GetComponentInParent<IInteractable>();

        if (interactable == null) return;

        interactable.Interact(_inventory);
    }
}




