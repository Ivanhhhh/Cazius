using System.Collections;
using UnityEngine;

public class PurgatoryDoor : MonoBehaviour, IEInteractable
{
    [SerializeField] private bool _isOpen;
    //[SerializeField] private float _openAngle;
    [SerializeField] private string _interactText = "You need Purgatory Key";

    [Header("Open The Door")]
    [SerializeField] private OpenTheDoor _openTheDoor;

    [SerializeField] private Transform _interactionUIPoint;


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
            if (KeyInventorySystem.Instance.HasPurgatoryKey)
            {
                _openTheDoor.OpenDoor(default);
            }
        }
    }

    public string GetInteractText()
    {
        return _interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsLocked()
    {
        if (KeyInventorySystem.Instance.HasPurgatoryKey)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}