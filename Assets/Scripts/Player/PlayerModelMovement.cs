using UnityEngine;

public class PlayerModelMovement : MonoBehaviour
{
    [SerializeField] Transform _modelPos;

    [Header("GlobalSettings")]

    public bool forceLookForward = false;

    private Transform _foreLookTowardsTransform;

    [SerializeField] float _forceLookOffset = 30f;

    [Header("AimingParams")]

    [SerializeField] float _aimSmoothTime = 0.1f;

    private bool _isAiming = false;

    [Header("LookingParams")]

    [SerializeField] float _maxAngle = 60f;

    [SerializeField] float _rotationOffset = 10f;

    [SerializeField] float _rotationSpeed = 3f;

    private bool _shouldRotateModel = false;

    private void LateUpdate()
    {
        transform.position = _modelPos.position;

        if (!forceLookForward)
        {


            if (_isAiming)
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
}
