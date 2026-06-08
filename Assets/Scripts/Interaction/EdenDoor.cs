using UnityEngine;
using System.Collections;

public class EdenDoor : MonoBehaviour, IEInteractable
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _openAngle;
    [SerializeField] private string _interactText = "You need Eden Key";

    [SerializeField] private Key _key;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private string _animationName;

    public void Interact(Transform interactorTransform)
    {
        if (_isOpen) return;

        if (KeyInventorySystem.Instance.HasEdenKey)
        {
            StartCoroutine(OpenDoor());
        }
    }

    public IEnumerator OpenDoor()
    {
        _playerMovement.enabled = false;

        yield return StartCoroutine(_key.WhenKeyOpenDoor(transform, _animationName));

        transform.Rotate(0, _openAngle, 0);
        _isOpen = true;

        _playerMovement.enabled = true;
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}
