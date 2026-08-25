using UnityEngine;

public class PickableItem : MonoBehaviour, IEInteractable
{
    [SerializeField] private ItemData _itemScriptableObject;
    [SerializeField] private string _interactText = "F to Take Cat";
    [SerializeField] private SFXManager.SFXCategoryType sfxType;

    public void Interact(Transform interactorTransform)
    {
        Inventory.Instance.AddItem(_itemScriptableObject);

        SFXManager.Instance.PlaySFXAtPosition(sfxType, transform.position);

        Destroy(gameObject);
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
