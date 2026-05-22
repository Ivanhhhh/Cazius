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
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] HingeJoint _joint;
    JointLimits limits;
    [SerializeField] string OpenDoorText = "F To Interact";
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //        IsNear = true;
    //        // OpenIsEnabled = true;
    //        GetInteractText();
    //    }
    //}
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //        _rigidbody.isKinematic = true;

    //    }
    //}
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //    }
    //}

    void Start()
    {
        limits = _joint.limits;
        _rigidbody.isKinematic = true;

    }
    void Update()
    {

        //else if (OpenIsEnabled == false && _isOpening == false) _joint.useMotor = false;
    }
    public IEnumerator OpenDoorMethod()
    {
        _rigidbody.isKinematic = false;

        _CanOpen = false;

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
        _rigidbody.isKinematic = false;

        _CanOpen = false;

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
        // StartCoroutine(OpenDoorMethod());     
        if (_CanOpen && !_canClose)
        {

            StartCoroutine(OpenDoorMethod());
        }
        else if (_canClose && !_CanOpen && !_isOpening)
        {

            StartCoroutine(CloseDoorMethod());
        }
    }
    //public void InteractClose(Transform interactorTransform)
    //{
    //    StartCoroutine(OpenDoorMethod());
    //}
    public string GetInteractText() { return OpenDoorText; }
    public Transform GetTransform()
    {
        return this.transform;
    }
}
