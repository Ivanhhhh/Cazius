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

    [Header("Door Type")]
    [SerializeField] private bool _isEdenDoor;
    [SerializeField] private bool _isPurgatoryDoor;

    void Start()
    {
        _lockIcon.SetActive(true);
        _unlockIcon.SetActive(false);
    }

    public void Interact(InventorySystem inventory)
    {
        if (_isOpen) return;

        if (_isEdenDoor)
        {
            if (!inventory.HasPurgatoryKey)
            {
                Debug.Log("You need PURGATORY key");
                return;
            }
        }

        if (_isPurgatoryDoor)
        {
            if (!inventory.HasEdenKey)
            {
                Debug.Log("You need EDEN key");
                return;
            }
        }

        OpenDoor();
    }

    private void OpenDoor()
    {
        _unlockIcon.SetActive(true);
        _lockIcon.SetActive(false);
        transform.Rotate(0, _openAngle, 0);
        _isOpen = true;

        Debug.Log("Door open");
    }
}
