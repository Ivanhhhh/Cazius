using UnityEngine;

public class InteractionRaycast : MonoBehaviour
{
    [SerializeField] private float _maxDistance;
    [SerializeField] private LayerMask _interactiveLayerMask;
    [SerializeField] private Transform _interactiveIcon;
    [SerializeField] private Vector3 _iconOffset;

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

            _interactiveIcon.gameObject.SetActive(true);

            _interactiveIcon.position = CurrentTarget.transform.position + _iconOffset;
        }
        else
        {
            CurrentTarget = null;
            _interactiveIcon.gameObject.SetActive(false);
        }
    }
}
