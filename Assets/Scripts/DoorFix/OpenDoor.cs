using System.Collections;
using UnityEngine;

public class OpenDoor : MonoBehaviour, IEInteractable
{
    private bool _canOpen = true;
    private bool _canClose = false;
    private bool _isOpening = false;

    [SerializeField] private HingeJoint _joint;
    [SerializeField] private string _interactText = "F to Open Door";

    private JointLimits _limits;

    private void Start()
    {
        _limits = _joint.limits;
    }

    public void Interact(Transform interactorTransform)
    {
        if (_canOpen && !_canClose)
        {
            StartCoroutine(OpenDoorMethod());
        }
        else if (_canClose && !_canOpen && !_isOpening)
        {
            StartCoroutine(CloseDoorMethod());
        }
    }

    private IEnumerator OpenDoorMethod()
    {
        _canOpen = false;
        _canClose = false;
        _isOpening = true;

        JointMotor motor = _joint.motor;
        motor.force = 150f;
        motor.targetVelocity = 600f;
        _joint.motor = motor;
        _joint.useMotor = true;
        _joint.useLimits = true;

        yield return new WaitForSeconds(1f);

        _isOpening = false;
        _canClose = true;
    }

    private IEnumerator CloseDoorMethod()
    {
        _canClose = false;

        JointMotor motor = _joint.motor;
        motor.force = 150f;
        motor.targetVelocity = -600f;
        _joint.motor = motor;
        _joint.useMotor = true;

        yield return new WaitForSeconds(1f);

        _canOpen = true;
    }

    public string GetInteractText() { return _interactText; }
    public Transform GetTransform() { return transform; }
}