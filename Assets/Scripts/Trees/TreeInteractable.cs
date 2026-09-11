using System.Collections.Generic;
using UnityEngine;

public class TreeInteractable : MonoBehaviour, IEInteractable
{
    [Header("Interaction")]
    [SerializeField] private string _interactText = "F to shake";
    [SerializeField] private SFXManager.SFXCategoryType _hitSFX;

    [Header("Drop")]
    [SerializeField] private List<Rigidbody> _objectsToDrop;

    private bool _dropped = false;

    [SerializeField] private Transform _interactionUIPoint;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    public void Interact(Transform interactorTransform)
    {
        SFXManager.Instance.PlaySFXAtPosition(_hitSFX, transform.position);

        if (_dropped) return;

        foreach (var rb in _objectsToDrop)
        {
            if (rb == null) continue;
            rb.transform.SetParent(null);
            rb.isKinematic = false;
        }

        _dropped = true;
    }

    public string GetInteractText() { return _interactText; }
    public Transform GetTransform() { return transform; }

    public bool IsLocked() { return false; }
}