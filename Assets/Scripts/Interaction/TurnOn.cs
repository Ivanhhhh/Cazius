using UnityEngine;

public class TurnOn : MonoBehaviour, IEInteractable
{
    [SerializeField] private string _interactText = "F to turn on";
    [SerializeField] private SFXManager.SFXCategoryType sfxType;
    [SerializeField] private Transform _interactionUIPoint;

    public void Interact(Transform interactorTransform)
    {
        SFXManager.Instance.PlaySFXAtPosition(sfxType, transform.position);
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