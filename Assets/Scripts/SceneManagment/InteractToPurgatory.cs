using UnityEngine;

public class InteractToPurgatory : MonoBehaviour, IEInteractable
{
    [SerializeField] private string _interactText = "F to Swap to Purgatory";
    [SerializeField] private SceneField[] scenesToLoad;

    public void Interact(Transform interactorTransform)
    {
        WorldChangeManager.Instance.SwapToPurgatory(scenesToLoad);
    }

    [SerializeField] private Transform _interactionUIPoint;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}
