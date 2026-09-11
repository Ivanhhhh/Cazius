using System.Collections.Generic;
using UnityEngine;

public class TreeInteractable : MonoBehaviour, IEInteractable
{
    [Header("Interaction")]
    [SerializeField] private string _interactText = "F to shake";
    [SerializeField] private ParticleSystem _leavesParticles;
    [SerializeField] private SFXManager.SFXCategoryType _hitSFX;
    [SerializeField] private SFXManager.SFXCategoryType _leavesSFX;
    [SerializeField] private SFXManager.SFXCategoryType _bounceSFX;

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
        if (_leavesParticles != null) { _leavesParticles.Play(); }
        SFXManager.Instance.PlaySFXAtPosition(_leavesSFX, transform.position);
        SFXManager.Instance.PlaySFXAtPosition(_bounceSFX, transform.position);
    }

    public string GetInteractText() { return _interactText; }
    public Transform GetTransform() { return transform; }

    public bool IsLocked() { return false; }
}