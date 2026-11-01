
using UnityEngine;

public class DiegeticUIFollowObject : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private Vector3 _rotationOffset = new Vector3(-90f, 0f, 0f);
    [SerializeField] private float _smoothTime = 0.1f;
    [SerializeField] private float _rotationSpeed = 10f;

    private Vector3 _velocity;

    private void Awake()
    {
        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_objectToFollow == null)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, _objectToFollow.position, ref _velocity, _smoothTime);

        if (_cameraTransform == null)
            return;

        Vector3 direction = transform.position - _cameraTransform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(_rotationOffset);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Exp(-_rotationSpeed * Time.deltaTime));
    }
}
