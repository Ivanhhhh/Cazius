using UnityEngine;

public class EdenDoor : MonoBehaviour, IEInteractable
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _openAngle;
    [SerializeField] private string _interactText = "You need Eden Key";

    public void Interact(Transform interactorTransform)
    {
        if (_isOpen) return;

        if (KeyInventorySystem.Instance.HasPurgatoryKey)
        {
            transform.Rotate(0, _openAngle, 0);
            _isOpen = true;
        }
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}
