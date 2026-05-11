using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference _inventoryAction;
    [SerializeField] private GameObject _inventoryCanvas;

    void OnEnable()
    {
        _inventoryAction.action.Enable();
        _inventoryAction.action.performed += OnInventory;
    }

    void OnDisable() => _inventoryAction.action.performed -= OnInventory;

    private void OnInventory(InputAction.CallbackContext _)
    {
        if (_inventoryCanvas.activeSelf)
        {
            PauseManager.Instance.Toggle();
            _inventoryCanvas.SetActive(false);
            return;
        }

        if (PauseManager.Instance.IsPaused)
            return;

        PauseManager.Instance.Toggle();
        _inventoryCanvas.SetActive(true);
    }

}
