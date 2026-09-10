using UnityEngine;

public class PlayerModelMovement : MonoBehaviour
{
    [SerializeField] Transform _modelPos;

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

        if (_isAiming)
        {

            AimingSmooth();

        }
        else
        {

            LookingSmooth();

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
        Debug.Log("ROTATE SMOOTH IS RUNNING");

        transform.rotation = Quaternion.Slerp(transform.rotation, _modelPos.rotation, Time.deltaTime * _rotationSpeed);

        Debug.Log(
    $"Current: {transform.eulerAngles.y:F2} | " +
    $"Target: {_modelPos.eulerAngles.y:F2}"
);

        if (AngleDifference(transform, _modelPos) < _rotationOffset)
        {
            _shouldRotateModel = false;
        }
    }

    private void ShouldRotate()
    {
        float rotationDiff = AngleDifference(transform, _modelPos);

        //Debug.Log(rotationDiff);

        if (rotationDiff >  _maxAngle)
        {
            _shouldRotateModel = true;
        }
    }

    private void AimingSmooth()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _modelPos.rotation, _aimSmoothTime);
    }

    private float AngleDifference (Transform a, Transform b)
    {
       // return  Mathf.Abs(a.eulerAngles.y - b.eulerAngles.y);

        return Mathf.Abs(Mathf.DeltaAngle(a.eulerAngles.y, b.eulerAngles.y));
    }
}
