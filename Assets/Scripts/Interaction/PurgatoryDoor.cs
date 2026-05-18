using UnityEngine;

public class PurgatoryDoor : MonoBehaviour, IEInteractable
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _openAngle;
    [SerializeField] private string _interactText = "You need Purgatory Key";

    public void Interact(Transform interactorTransform)
    {
        if (_isOpen) return;

        if (KeyInventorySystem.Instance.HasEdenKey)
        {
            transform.Rotate(0, _openAngle, 0);
            _isOpen = true;
        }
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}