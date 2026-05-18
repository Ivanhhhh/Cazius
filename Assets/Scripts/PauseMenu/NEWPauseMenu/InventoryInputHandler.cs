using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InventoryInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference _inventoryAction;
    
    public static event Action<bool> OnInventoryToggled;
    private bool _isInventoryOpen = false; 

    void OnEnable()
    {
        _inventoryAction.action.Enable();
        _inventoryAction.action.performed += OnInventory;
    }

    void OnDisable() => _inventoryAction.action.performed -= OnInventory;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            Debug.Log(">>> [Update] Teclado físico detectó TAB.");
            
            // Forzamos la ejecución de la función
            OnInventory(default); 
        }
    }

    private void OnInventory(InputAction.CallbackContext _)
    {
        Debug.Log("1. Intentando procesar inventario. Estado actual abierto: " + _isInventoryOpen);

        if (_isInventoryOpen)
        {
            Debug.Log("2. Cerrando inventario...");
            PauseManager.Instance.Toggle();
            _isInventoryOpen = false; 
            
            OnInventoryToggled?.Invoke(false); 
            return;
        }

        if (PauseManager.Instance.IsPaused)
        {
            Debug.Log("X. Juego pausado por otra razón, ignorando.");
            return;
        }

        Debug.Log("A. Abriendo inventario...");
        PauseManager.Instance.Toggle();
        _isInventoryOpen = true; 
        OnInventoryToggled?.Invoke(true); 
    }
}