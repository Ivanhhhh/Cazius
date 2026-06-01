using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class OpenDoor : MonoBehaviour, IEInteractable
{
    private bool _CanOpen = true;
    private bool _canClose = false;
    private bool Flag = false;
    private bool _isOpening = false;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Animator _Anim;
    [SerializeField] string OpenDoorText = "F To Interact";
   
    public IEnumerator OpenDoorMethod()
    {
        _Anim.SetTrigger("OpenDoor");

        _CanOpen = false;     
        _isOpening = true;
        _canClose = false;

        yield return new WaitForSeconds(1f);
        _isOpening = false;
        _canClose = true;     
        _CanOpen = false;
        Flag = true;


        print("Open");
    }
    public IEnumerator CloseDoorMethod()
    {
        _Anim.SetTrigger("CloseDoor");
        yield return new WaitForSeconds(1f);
        _CanOpen = true;
        //_isOpening = false;
        _canClose = false;
        print("Close");
        Flag = false;
    }
    public void Interact(Transform interactorTransform)
    {
        print("Interact llamado");   // ← agregá esto

        if (_CanOpen && !_canClose && Flag == false)
        {

            StartCoroutine(OpenDoorMethod());
        }
        else if (_canClose && !_CanOpen && !_isOpening && Flag == true)
        {

            StartCoroutine(CloseDoorMethod());
        }
    }
    
    public string GetInteractText() { return OpenDoorText; }
    public Transform GetTransform()
    {
        return this.transform;
    }
}
