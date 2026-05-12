using UnityEngine;
using UnityEngine.InputSystem;
using System; // ¡NUEVO: Necesario para usar Action!

public class InventoryInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference _inventoryAction;
    [SerializeField] private GameObject _inventoryCanvas;

    // EVENTO OBSERVER: Cualquier script puede suscribirse a esto
    public static event Action<bool> OnInventoryToggled;

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
            
            // Avisamos que el inventario se cerró
            OnInventoryToggled?.Invoke(false); 
            return;
        }

        if (PauseManager.Instance.IsPaused)
            return;

        PauseManager.Instance.Toggle();
        _inventoryCanvas.SetActive(true);
        
        // Avisamos que el inventario se abrió
        OnInventoryToggled?.Invoke(true); 
    }
}