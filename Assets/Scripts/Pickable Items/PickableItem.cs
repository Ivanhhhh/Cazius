using UnityEngine;

public class PickableItem : MonoBehaviour, IEInteractable
{
    [SerializeField] private ItemData _itemScriptableObject;

    [Header("Cantidad Predeterminada (Para objetos en la escena)")]
    [Tooltip("Si es mayor a 0, sobrescribe el 'value' del ItemData para este objeto específico en la escena.")]
    [SerializeField] private int _quantityOverride = -1;

    [SerializeField] private string _interactText = "F to Take";
    [SerializeField] private SFXManager.SFXCategoryType sfxType;

    private void Awake()
    {
        // Si el diseñador puso una cantidad específica en este objeto de la escena, la usamos
        if (_quantityOverride > 0 && _itemScriptableObject != null)
        {
            _itemScriptableObject = Instantiate(_itemScriptableObject);
            _itemScriptableObject.value = _quantityOverride;
        }

        UpdateInteractText();
    }

    // Este método lo llama Inventory.cs cuando dropeas un objeto
    public void Setup(ItemData item)
    {
        _itemScriptableObject = item;
        UpdateInteractText();
    }

    private void UpdateInteractText()
    {
        if (_itemScriptableObject != null)
        {
            if (_itemScriptableObject.value > 1)
                _interactText = $"F to Take {_itemScriptableObject.value}x {_itemScriptableObject.name}";
            else
                _interactText = $"F to Take {_itemScriptableObject.name}";
        }
    }

    public void Interact(Transform interactorTransform)
    {
        if (_itemScriptableObject == null) return;

        // AddItem suma exactamente la cantidad que tenga el ItemData
        Inventory.Instance.AddItem(_itemScriptableObject);

        SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(
            sfxType,
            transform.position
        );

        Destroy(gameObject);
    }

    [SerializeField] private Transform _interactionUIPoint;

    public Transform GetInteractionUIPoint() => _interactionUIPoint != null ? _interactionUIPoint : transform;
    public string GetInteractText() => _interactText;
    public Transform GetTransform() => transform;
    public bool IsLocked() => false;
}