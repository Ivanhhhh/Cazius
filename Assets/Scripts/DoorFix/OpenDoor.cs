using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenDoor : MonoBehaviour
{ private bool OpenIsEnabled = false;

    private bool _isOpening = false;


    [SerializeField] HingeJoint _joint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OpenIsEnabled = true;

            print("entro");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OpenIsEnabled = false;
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_joint = GetComponent<HingeJoint>();

    }

    // Update is called once per frame
    void Update()
    {
       if (Keyboard.current.fKey.wasPressedThisFrame && OpenIsEnabled == true && _isOpening == false)
       {
            StartCoroutine(Timer());
           // _joint.useMotor = true;

       }
       else if (OpenIsEnabled == false && _isOpening == false) _joint.useMotor = false;

    }

    public IEnumerator Timer ()
    {
        _isOpening = true; 

        _joint.useMotor = true;
        _joint.useLimits = true;


        yield return new WaitForSeconds(8f);

        _joint.useMotor = false;
        _joint.useLimits = false;

        _isOpening = false;  // desbloquea


    }
}
