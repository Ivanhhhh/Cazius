using System.Collections;
using UnityEngine;

public class OpenDoorWithItem : MonoBehaviour, IEInteractable
{
    [Header("References")]
    [SerializeField] SideOpen _SideOpen;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Animator _Anim;

    [Header("Config")]
    [SerializeField] string _itemIDNeededToOpen;
    [SerializeField] string OpenDoorText = "F To Open/Close";
    [SerializeField] float _coolDown = 0.15f;

    private Inventory _inventory;
    private bool _openedWithItem = false;
    private bool _canClose = false;
    private bool IsRunning;

    private void Start()
    {
        _inventory = Inventory.Instance;
    }

    public void Interact(Transform interactorTransform)
    {
        if (IsRunning) return;

        // Item Checks
        if (!_openedWithItem)
        {
            if (_inventory == null) { _inventory = Inventory.Instance; }

            if (_inventory.HasItem(_itemIDNeededToOpen))
            {
                _inventory.RemoveItem(_itemIDNeededToOpen);
                _openedWithItem = true; // Door is now permanently unlocked
            }
            else { return; }
        }

        // Open/Close behavior
        if (_canClose) { StartCoroutine(CloseDoorMethod()); }
        else if (_SideOpen.Opened) { StartCoroutine(OpenDoorMethodOtherSide()); }
        else { StartCoroutine(OpenDoorMethod()); }
    }

    public IEnumerator OpenDoorMethod()
    {
        IsRunning = true;
        _Anim.SetTrigger("OpenDoor");

        yield return new WaitForSeconds(_coolDown);

        _canClose = true;
        IsRunning = false;
    }

    public IEnumerator OpenDoorMethodOtherSide()
    {
        IsRunning = true;

        _Anim.SetTrigger("OpenDoor2");

        yield return new WaitForSeconds(_coolDown);
        _canClose = true;

        IsRunning = false;
    }

    public IEnumerator CloseDoorMethod()
    {
        IsRunning = true;
        _Anim.SetTrigger("CloseDoor");

        yield return new WaitForSeconds(_coolDown);

        _canClose = false;
        IsRunning = false;
    }

    public string GetInteractText() { return OpenDoorText; }
    public Transform GetTransform() { return this.transform; }

}
