using UnityEngine;

public class PickableItem : MonoBehaviour, IEInteractable
{
    [SerializeField] private ItemData _itemScriptableObject;
    [SerializeField] private string _interactText = "F to Pick Scrap";

    public void Interact(Transform interactorTransform)
    {
        Inventory.Instance.AddItem(_itemScriptableObject);

        SFXManager.Instance.PlaySFXAtPosition(
            SFXManager.SFXCategoryType.RechargingGun, transform.position);

        Destroy(gameObject);
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }
}
