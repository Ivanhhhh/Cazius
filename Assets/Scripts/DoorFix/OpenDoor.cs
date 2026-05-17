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

    [SerializeField] GameObject OpenDoorText;

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
                StartCoroutine(CloseDoor());
            }
        }

        //else if (OpenIsEnabled == false && _isOpening == false) _joint.useMotor = false;

    }

    public IEnumerator Timer()
    {
        _CanOpen = false;
        _isOpening = true;
        _joint.useMotor = true;
        _joint.useLimits = true;
        _canClose = false;     // ← no puede cerrar mientras abre
        yield return new WaitForSeconds(1f);
        _isOpening = false;
        _canClose = true;      // ← recién ahora puede cerrar
    }


    public IEnumerator CloseDoor()
    {
        _canClose = false;

        _joint.useMotor = false;
        _joint.useLimits = false;
        //  _isOpening = false;

        yield return new WaitForSeconds(2);
        _CanOpen = true;

    }


    public void Interact(Transform interactorTransform)
    {
       _CanOpen = false;
        StartCoroutine(Timer());



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

