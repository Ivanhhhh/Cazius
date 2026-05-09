using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _openAngle = -90f;

    [Header("UI")]
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private GameObject _unlockIcon;

    public void Interact(InventorySystem inventory)
    {
        if (_isOpen) return;

        if (!inventory.HasKey)
        {
            ShowLockedUI();
            Debug.Log("Door locked");

            return;
        }

        OpenDoor();
    }
    private void ShowLockedUI()
    {
        _lockIcon.SetActive(true);
    }
    private void OpenDoor()
    {
        transform.Rotate(0, _openAngle, 0);
        _isOpen = true;
    }
}
