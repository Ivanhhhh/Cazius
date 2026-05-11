using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float _interactRange = 1f;
    [SerializeField] private float _interactAngle = 60f;
    private PlayerControls _controls;

    private void Awake()
    {
        _controls = new PlayerControls();
        _controls.Player.Interact.performed += _ => TryInteract();
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    private void TryInteract()
    {
        IInteractable interactable = GetInteractableObject();
        if (interactable != null)
            interactable.Interact(transform);
    }

    public IInteractable GetInteractableObject()
    {
        List<IInteractable> interactableList = new List<IInteractable>();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, _interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                Vector3 dir = collider.transform.position - transform.position;
                if (Vector3.Angle(transform.forward, dir) < _interactAngle)
                {
                    interactableList.Add(interactable);
                }
            }
        }

        IInteractable closestInteractable = null;
        foreach (IInteractable interactable in interactableList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if ((Vector3.Distance(transform.position, interactable.GetTransform().position))
                    < (Vector3.Distance(transform.position, closestInteractable.GetTransform().position)))
                {
                    closestInteractable = interactable;
                }
            }
        }

        return closestInteractable;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactRange);

        Vector3 leftDir = Quaternion.Euler(0, -_interactAngle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, _interactAngle, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftDir * _interactRange);
        Gizmos.DrawRay(transform.position, rightDir * _interactRange);
        Gizmos.DrawRay(transform.position, transform.forward * _interactRange);
    }

}
