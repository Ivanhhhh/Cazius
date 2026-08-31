using System.Collections;
using UnityEngine;

public class PurgatoryDoor : MonoBehaviour, IEInteractable
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _openAngle;
    [SerializeField] private string _interactText = "You need Purgatory Key";

    [Header("Open Door Anim")]
    [SerializeField] private WhenOpenDoor _whenOpenDoor;

    [SerializeField] private Transform _interactionUIPoint;

    private PlayerMovement player;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    public void Interact(Transform interactorTransform)
    {
        if (_isOpen)
            return;

        if (KeyInventorySystem.Instance.HasPurgatoryKey)
        {
            StartCoroutine(OpenDoor());
        }
    }

    public IEnumerator OpenDoor()
    {
        player.enabled = false;

        SFXManager.Instance.PlaySFXAtPosition(
            SFXManager.SFXCategoryType.DoorPurgatory,
            transform.position
        );

        yield return StartCoroutine(
            _whenOpenDoor.WhenKeyOpenDoor("PurgatoryDoor")
        );

        transform.Rotate(0, _openAngle, 0);

        _isOpen = true;

        player.enabled = true;
    }

    public string GetInteractText()
    {
        return _interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}