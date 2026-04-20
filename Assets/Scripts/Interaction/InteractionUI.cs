using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Update()
    {
        transform.LookAt(target);
    }
}
