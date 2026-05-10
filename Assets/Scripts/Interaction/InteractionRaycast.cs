using UnityEngine;

public class InteractionRaycast : MonoBehaviour // Lo tiene el raycast 
{
    [SerializeField] private float _maxDistance;
    [SerializeField] private LayerMask _interactiveLayerMask;

    public GameObject CurrentTarget { get; private set; }

    private void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        RaycastHit hit;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * _maxDistance, Color.red);

        if (Physics.Raycast(origin, direction, out hit, _maxDistance, _interactiveLayerMask))
        {
            CurrentTarget = hit.transform.gameObject;
        }
        else
        {
            CurrentTarget = null;
        }
    }
}
