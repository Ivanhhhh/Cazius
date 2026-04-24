using UnityEngine;

public class BasicRotate : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private Vector3 direction = Vector3.up;

    void Update()
    {
        transform.Rotate(direction.normalized * speed * Time.deltaTime);
    }
}