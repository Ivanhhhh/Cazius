using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenDoor : MonoBehaviour, IEInteractable
{
    //private bool OpenIsEnabled = false;

    //private bool _isOpening = false;

    private bool _CanOpen = true;

    private bool _canClose = false;

    private bool IsNear;

    private bool _isOpening = false;

   

    [SerializeField] HingeJoint _joint;

    JointLimits limits;

   [SerializeField] GameObject OpenDoorText;

    [SerializeField] Vector3 CloseAxis;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IsNear = true;

            // OpenIsEnabled = true;

            GetInteractText();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IsNear = true;


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OpenDoorText.SetActive(false);

            IsNear = false;
        }

    }
    void Start()
    {
        //_joint = GetComponent<HingeJoint>();
         limits = _joint.limits;


    }

    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.fKey.wasPressedThisFrame && IsNear == true)
        {
            if (_CanOpen && !_canClose)
            {
                Interact(this.transform);
                OpenDoorText.SetActive(false);
            }
            else if (_canClose && !_CanOpen && !_isOpening)
            {
                StartCoroutine(CloseDoorMethod());
            }
        }

        //else if (OpenIsEnabled == false && _isOpening == false) _joint.useMotor = false;

    }

    public IEnumerator OpenDoorMethod()
    {
        JointMotor motor = _joint.motor;
        motor.force = 150f;
        motor.targetVelocity = 600f;  // negativo
        _joint.motor = motor;


        _isOpening = true;
        _joint.useMotor = true;
        _joint.useLimits = true;
        _canClose = false;     // ← no puede cerrar mientras abre
       

        yield return new WaitForSeconds(1f);
        _isOpening = false;
        _canClose = true;      // ← recién ahora puede cerrar
        _CanOpen = false;

    }


    public IEnumerator CloseDoorMethod()
    {


        JointMotor motor = _joint.motor;
        motor.force = 150f;
        motor.targetVelocity = -600f;  // negativo
        _joint.motor = motor;
        _joint.useMotor = true;



        //_joint.useMotor = false;
        //_joint.useLimits = false;


        yield return new WaitForSeconds(1f);
        _CanOpen = true;
        _canClose = false;
       // _isOpening = true;    // ← recién ahora puede abrir
    }


    public void Interact(Transform interactorTransform)
    {
       _CanOpen = false;
        StartCoroutine(OpenDoorMethod());



    }

    public void InteractClose(Transform interactorTransform)
    {
        _CanOpen = false;
        StartCoroutine(OpenDoorMethod());



    }
    public string GetInteractText()
    {
        OpenDoorText.SetActive(true);
        return OpenDoorText.ToString();
    }
    public Transform GetTransform()
    {
        return this.transform;
    }
}

