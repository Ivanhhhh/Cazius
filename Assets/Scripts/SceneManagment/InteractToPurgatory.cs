using UnityEngine;

public class InteractToPurgatory : MonoBehaviour, IEInteractable
{
    [SerializeField] private string _interactText = "F to Swap to Purgatory";
    [SerializeField] private SceneField[] scenesToLoad;

    public void Interact(Transform interactorTransform)
    {
        WorldChangeManager.Instance.SwapToPurgatory(scenesToLoad);
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}
