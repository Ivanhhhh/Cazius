using UnityEngine;

public class BasicFloat : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float height = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * speed) * height;
    }
}
