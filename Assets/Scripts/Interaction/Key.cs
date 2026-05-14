using UnityEngine;

public class Key : MonoBehaviour, IInteractable // Lo tiene la "Key" con layer "Interactive", _keyPanel tiene q estar apagado
{
    [Header("Key type")]
    [SerializeField] private bool _isEdenKey;
    [SerializeField] private bool _isPurgatoryKey;

    [Header("UI")]
    [SerializeField] private GameObject _keyPanel;
    [SerializeField] private GameObject _interactionIcon;

    [SerializeField] private ObjectsActivator _objectsToActivate;

    public void Interact(InventorySystem inventory)
    {
        if (_isPurgatoryKey)
        {
            inventory.AddPurgatoryKey();

            if (_objectsToActivate != null)
            {
                _objectsToActivate.Interact(inventory);
            }
        }

        if (_isEdenKey)
        {
            inventory.AddEdenKey();
        }

        _keyPanel.SetActive(true);
        _interactionIcon.SetActive(false);
        gameObject.SetActive(false);
    }
}
