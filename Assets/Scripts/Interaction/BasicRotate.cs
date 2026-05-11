using UnityEngine;

public class BasicRotate : MonoBehaviour // Lo tiene la "Key" con layer "Interactive"
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Vector3 _directionRotate = Vector3.up;

    void Update()
    {
        transform.Rotate(_directionRotate.normalized * _speed * Time.deltaTime);
    }
}