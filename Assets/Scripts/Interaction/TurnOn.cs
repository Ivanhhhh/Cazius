using UnityEngine;

public class TurnOn : MonoBehaviour, IEInteractable
{
    [SerializeField] private string _interactText = "F to turn on";
    [SerializeField] private SFXManager.SFXCategoryType sfxType;
    [SerializeField] private Transform _interactionUIPoint;
    [SerializeField] private Collider _interactCollider;

    public void Interact(Transform interactorTransform)
    {
        SFXManager.Instance.PlaySFXAtPosition(sfxType, transform.position);
        if (_interactCollider != null ) {  _interactCollider.enabled = false; }
    }

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

    public bool IsLocked()
    {
        return false;
    }
}