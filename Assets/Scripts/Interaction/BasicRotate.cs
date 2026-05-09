using UnityEngine;

public class BasicRotate : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Vector3 _direction = Vector3.up;

    void Update()
    {
        transform.Rotate(_direction.normalized * _speed * Time.deltaTime);
    }
}