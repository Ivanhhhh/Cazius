using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Door : MonoBehaviour, IInteractable  // Lo tienen "Door Pivot" con tag "Door" y layer "Interactive"
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _openAngle;

    [Header("UI")] // Tienen q estar apagados
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private GameObject _unlockIcon;

    void Start()
    {
        _lockIcon.SetActive(true);
        _unlockIcon.SetActive(false);
    }

    public void Interact(InventorySystem inventory)
    {
        if (_isOpen) return;

        if (!inventory.HasKey)
        {
            Debug.Log("Door locked");
            return;
        }

        OpenDoor();
    }

    private void OpenDoor()
    {
        _unlockIcon.SetActive(true);
        _lockIcon.SetActive(false);
        transform.Rotate(0, _openAngle, 0);
        _isOpen = true;
    }
}
