using UnityEngine;

public class Door : MonoBehaviour, IEInteractable  
    // Lo tienen "Door Pivot" con tag "Door" y layer "Interactive"
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _openAngle;

    [Header("UI")] // Tienen q estar apagados
    //[SerializeField] private GameObject _lockIcon;
    //[SerializeField] private GameObject _unlockIcon;
    [SerializeField] private string _interactTextA = "F to Open Door";
    [SerializeField] private string _interactTextB = "You need PURGATORY key";
    [SerializeField] private string _interactTextC = "You need EDEN key";

    private string _interactText;

    [Header("Door Type")]
    [SerializeField] private bool _isEdenDoor;
    [SerializeField] private bool _isPurgatoryDoor;

    void Start()
    {
        //_lockIcon.SetActive(true);
        //_unlockIcon.SetActive(false);
    }
    public void Interact(Transform interactorTransform)
    {
        if (_isOpen) return;

        if (_isEdenDoor)
        {
            if (!KeyInventorySystem.Instance.HasPurgatoryKey)
            {
                Debug.Log("You need PURGATORY key");
                _interactText = _interactTextA;
                return;
            }
        }

        if (_isPurgatoryDoor)
        {
            if (!KeyInventorySystem.Instance.HasEdenKey)
            {
                Debug.Log("You need EDEN key");
                _interactText = _interactTextB;
                return;
            }
        }

        OpenDoor();
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

    private void OpenDoor()
    {
        Debug.Log("#### OpenDoor()");

        //_unlockIcon.SetActive(true);
        //_lockIcon.SetActive(false);

        transform.Rotate(0, _openAngle, 0);
        _isOpen = true;

        Debug.Log("Door open");
    }

    /*public void Interact(KeyInventorySystem inventory)
    {
        Debug.Log("#### IsOpen: " + _isOpen);
        if (_isOpen) return;

        if (_isEdenDoor)
        {
            if (!inventory.HasPurgatoryKey)
            {
                Debug.Log("You need PURGATORY key");
                return;
            }
        }

        if (_isPurgatoryDoor)
        {
            if (!inventory.HasEdenKey)
            {
                Debug.Log("You need EDEN key");
                return;
            }
        }

        OpenDoor();
    }*/

}
