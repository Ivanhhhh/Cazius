using UnityEngine;

public class DiegeticUIFollowObject : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;

    [SerializeField] private float _smoothTime = 0.1f;
    [SerializeField] private float _rotationSpeed = 10f;

    private Vector3 _velocity;

    private void LateUpdate()
    {
        if (_objectToFollow == null)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, _objectToFollow.position, ref _velocity, _smoothTime );

        transform.rotation = Quaternion.Slerp(transform.rotation, _objectToFollow.rotation, 1f - Mathf.Exp(-_rotationSpeed * Time.deltaTime)
        );
    }
}