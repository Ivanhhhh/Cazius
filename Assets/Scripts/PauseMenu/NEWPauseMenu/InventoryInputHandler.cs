using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InventoryInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference _inventoryAction;
    [SerializeField] private GameObject _inventoryCanvas;

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
            
            OnInventoryToggled?.Invoke(false); 
            return;
        }

        if (PauseManager.Instance.IsPaused)
            return;

        PauseManager.Instance.Toggle();
        _inventoryCanvas.SetActive(true);
        
        OnInventoryToggled?.Invoke(true); 
    }
}