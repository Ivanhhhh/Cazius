using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class OpenDoor : MonoBehaviour, IEInteractable
{
    [SerializeField] SideOpen _SideOpen;
    private bool _canClose = false;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Animator _Anim;
    [SerializeField] string OpenDoorText = "F To Interact";
    private bool IsRunning;
    public IEnumerator OpenDoorMethod()
    {  
        IsRunning = true;
       _Anim.SetTrigger("OpenDoor");
      
        yield return new WaitForSeconds(1f);
        _canClose = true;     
        
        print("Open1");

        IsRunning = false;
    }

    public IEnumerator OpenDoorMethodOtherSide()
    {
        IsRunning = true;

        _Anim.SetTrigger("OpenDoor2");
      
        yield return new WaitForSeconds(1f);
        _canClose = true;
        
        print("Open2");
        IsRunning = false;

    }

    public IEnumerator CloseDoorMethod()
    {
        IsRunning = true;

        _Anim.SetTrigger("CloseDoor");
        yield return new WaitForSeconds(1f);
        _canClose = false;
        print("Close3");
        IsRunning = false;
    }
    public void Interact(Transform interactorTransform)
    {
        if (IsRunning) return;
        if (_SideOpen.Opened == true && _canClose == false)
        {

            StartCoroutine(OpenDoorMethodOtherSide());
        }

        else if (_SideOpen.Opened == false && _canClose== false)
        {
            StartCoroutine(OpenDoorMethod());         
        }

        else if (_canClose == true)
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
