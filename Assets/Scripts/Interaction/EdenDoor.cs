using UnityEngine;
using System.Collections;

public class EdenDoor : MonoBehaviour, IEInteractable
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _openAngle;
    [SerializeField] private string _interactText = "You need Eden Key";

    [Header("Open Door Anim")]
    [SerializeField] private WhenOpenDoor _whenOpenDoor;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Animator _playerAnimator;

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
        _playerAnimator.speed = 0f;

        yield return StartCoroutine(_whenOpenDoor.WhenKeyOpenDoor("EdenDoor"));

        transform.Rotate(0, _openAngle, 0);
        _isOpen = true;

        _playerMovement.enabled = true;
        _playerAnimator.speed = 1f;
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}
