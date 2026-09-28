using UnityEngine;

public class RecorderInteraction : MonoBehaviour, IEInteractable
{
    [SerializeField] private string _interactText = "F to Take Recorder";
    [SerializeField] private SFXManager.SFXCategoryType sfxType;

    [SerializeField] private Transform _interactionUIPoint;

    public void Interact(Transform interactorTransform)
    {
        SFXManager.Instance.PlaySFXAtPosition(sfxType, transform.position);

        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        return _interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    public bool IsLocked()
    {
        return false;
    }
}
