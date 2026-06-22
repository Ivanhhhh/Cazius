using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Key : MonoBehaviour, IEInteractable
{
    [Header("Key type")]
    [SerializeField] private bool _isEdenKey;
    [SerializeField] private bool _isPurgatoryKey;

    [Header("UI")]
    [SerializeField] private GameObject _keyPanel;
    [SerializeField] private string _interactText = "F to Grab Key";

    [SerializeField] private ObjectsActivator _objectsToActivate;

    public void Interact(Transform interactorTransform)
    {
        if (_isPurgatoryKey)
        {
            KeyInventorySystem.Instance.AddPurgatoryKey();
        }

        if (_isEdenKey)
        {
            KeyInventorySystem.Instance.AddEdenKey();
            if (_objectsToActivate != null)
            {
                _objectsToActivate.Activate();
            }
        }

        _keyPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}
