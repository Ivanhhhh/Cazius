using GLTFast.Schema;
using UnityEngine;
using Camera = UnityEngine.Camera;

public class PlayerModelMovement : MonoBehaviour
{
    [SerializeField] Transform _modelPos;
    [SerializeField] Rigidbody _rb;
    [SerializeField] Camera _camera;

    [Header("GlobalSettings")]

    public Vector3 modelOffset;

    public bool forceLookForward = false;

    private Transform _foreLookTowardsTransform;

    [SerializeField] float _forceLookOffset = 30f;

    [Header("AimingParams")]

    [SerializeField] float _aimSmoothTime = 0.1f;

    public bool isAiming = false;

    [Header("LookingParams")]

    [SerializeField] float _maxAngle = 60f;

    [SerializeField] float _rotationOffset = 10f;

    [SerializeField] float _rotationSpeed = 3f;

    private bool _shouldRotateModel = false;

    [Header("WalkingParams")]

    [SerializeField] float _walkingRotationSpeed = 1f;

    [SerializeField] float _walkingRotateMaxAngle = 20f;
    [SerializeField] float _walkingRotateHalfAngle = 15f;

    private void LateUpdate()
    {
        transform.position = _modelPos.position - modelOffset;

        if (!forceLookForward)
        {


            if (isAiming)
            {

                AimingSmooth();

            }
            else
            {

                LookingSmooth();

            }

        }
        else
        {
            LookForward();
        }

    }


    private void LookingSmooth()
    {

        Vector3 velocity = _rb.linearVelocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.01f)
        {
            WalkingRotateSmooth();
            return;
        }

        if (_shouldRotateModel)
        {
            RotateSmooth();
        }
        else
        {
            ShouldRotate();
        }


    }

    private void RotateSmooth()
    {

        transform.rotation = Quaternion.Slerp(transform.rotation, _modelPos.rotation, Time.deltaTime * _rotationSpeed);

        if (AngleDifference(transform, _modelPos) < _rotationOffset)
        {
            _shouldRotateModel = false;
        }
    }

    private void WalkingRotateSmooth()
    {
        //transform.rotation = Quaternion.Slerp(transform.rotation, _modelPos.rotation, Time.deltaTime * _walkingRotationSpeed);

        Vector3 moveDir = _rb.linearVelocity;
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude < 0.01f)
            return;

        moveDir.Normalize();

        Vector3 cameraForward = _camera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        float moveAngle = Vector3.SignedAngle(cameraForward, moveDir, Vector3.up);

        float rotationOffset = GetWalkingRotationOffset(moveAngle);

        Quaternion targetRotation = Quaternion.Euler(0f, _modelPos.eulerAngles.y + rotationOffset, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _walkingRotationSpeed);
    }
    /*
    private void RotateTowardsWalkingDir()
    {
        Vector3 moveDir = _rb.linearVelocity.normalized;

        Debug.Log(moveDir);

        Quaternion rotate;

        rotate = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 10);

        rotate.x = 0f;
        rotate.z = 0f;

        rotate.y = Mathf.Clamp(rotate.y, 0f, _walkingRotateMaxAngle);

            transform.rotation = rotate;
        
        if (IsMovingForwardLeft())
        {
            transform.rotation = rotate;
        }
        else
        {
            rotate.y = -rotate.y;
            transform.rotation = rotate;
        }
        

    }*/
/*
    private bool IsMovingForwardLeft()
    {
        Vector3 moveDir = _rb.linearVelocity;
        moveDir.y = 0f;
        moveDir.Normalize();

        Vector3 cameraForward = _camera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _camera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        float forwardDot = Vector3.Dot(moveDir, cameraForward);
        float rightDot = Vector3.Dot(moveDir, cameraRight);

        bool forwardOrLeft = forwardDot > 0f || rightDot < 0f;

       return forwardOrLeft;
    }*/

    private void RotateSmoothTowards()
    {

        Vector3 dir = _foreLookTowardsTransform.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        targetRotation.x = 0;
        targetRotation.z = 0;

        targetRotation *= Quaternion.Euler(0f, _forceLookOffset, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        if (AngleDifference(transform, _modelPos) < _rotationOffset)
        {
            _shouldRotateModel = false;
        }
    }

    private void ShouldRotate()
    {
        float rotationDiff = AngleDifference(transform, _modelPos);

        if (rotationDiff >  _maxAngle)
        {
            _shouldRotateModel = true;
        }
    }

    private void LookForward()
    {
        RotateSmoothTowards();
    }

    private void AimingSmooth()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _modelPos.rotation, _aimSmoothTime);
    }

    private float AngleDifference (Transform a, Transform b)
    {

        return Mathf.Abs(Mathf.DeltaAngle(a.eulerAngles.y, b.eulerAngles.y));
    }

    public void ForceToLookForward(bool force, Transform lookAT)
    {
        if (force)
        {
            forceLookForward = true;

            _foreLookTowardsTransform = lookAT;
        }
        else
        {
            forceLookForward = false;
        }
    }

    private float GetWalkingRotationOffset(float angle)
    {

        if (angle >= -22.5f && angle < 22.5f)
            return 0f;

        if (angle >= 22.5f && angle < 67.5f)
            return _walkingRotateHalfAngle;

        if (angle >= 67.5f && angle < 112.5f)
            return _walkingRotateMaxAngle;

        if (angle >= 112.5f && angle < 157.5f)
            return -_walkingRotateHalfAngle;

        if (angle >= 157.5f || angle < -157.5f)
            return 0f;

        if (angle >= -157.5f && angle < -112.5f)
            return _walkingRotateHalfAngle;

        if (angle >= -112.5f && angle < -67.5f)
            return -_walkingRotateMaxAngle;

        if (angle >= -67.5f && angle < -22.5f)
            return -_walkingRotateHalfAngle;

        return 0f;
    }
}
