using UnityEngine;

public class InteractToEden : MonoBehaviour, IEInteractable
{
    [SerializeField] private string _interactText = "F to Swap to Eden";
    [SerializeField] private SceneField[] scenesToLoad;

    [SerializeField] private Transform _interactionUIPoint;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    public void Interact(Transform interactorTransform)
    {
        WorldChangeManager.Instance.SwapToEden(scenesToLoad);
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}
