using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorFixer : MonoBehaviour
{
    private Animator _Anim;

    private void Start()
    {
        _Anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _Anim.SetTrigger("OpenDoor");
           // set trigger anim
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // set trigger anim
            _Anim.SetTrigger("CloseOpen");
        }
    }
   
}

