using UnityEngine;

public class FaceDirection : MonoBehaviour
{
    [SerializeField] private Vector3 targetDirection = Vector3.forward;

    void Update()
    {
        if (targetDirection != Vector3.zero)
        {
            transform.forward = targetDirection.normalized;
        }
    }
}
