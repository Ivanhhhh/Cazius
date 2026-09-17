using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float _interactRange = 10f;
    [SerializeField] private float _interactAngle = 60f;
    [SerializeField] private float _interactOffset = 5f;

    public static event Action<IEInteractable> OnInteractableChanged;

    private PlayerControls _controls;
    private IEInteractable _currentInteractable;

    private void Awake()
    {
        _controls = GameInputManager.Instance.Controls;
        _controls.Player.Interact.performed += _ => TryInteract();
    }

    private void Update()
    {
        IEInteractable best = GetInteractableObject();

        if (best != _currentInteractable)
        {
            _currentInteractable = best;
            OnInteractableChanged?.Invoke(_currentInteractable);
        }
    }

    private void TryInteract()
    {
        if (_currentInteractable != null)
            _currentInteractable.Interact(transform);
    }

    public IEInteractable GetInteractableObject()
    {
        List<IEInteractable> interactableList = new List<IEInteractable>();
        Vector3 origin = transform.position + transform.forward * _interactOffset;
        Collider[] colliderArray = Physics.OverlapSphere(origin, _interactRange);

        foreach (Collider collider in colliderArray)
        {
            IEInteractable interactable = collider.GetComponentInParent<IEInteractable>();

            if (interactable != null)
            {
                Vector3 dir = collider.transform.position - origin;

                if (Vector3.Angle(transform.forward, dir) < _interactAngle)
                {
                    interactableList.Add(interactable);
                }
            }
        }

        IEInteractable closestInteractable = null;
        foreach (IEInteractable interactable in interactableList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactable.GetTransform().position)
                    < Vector3.Distance(transform.position, closestInteractable.GetTransform().position))
                    closestInteractable = interactable;
            }
        }

        return closestInteractable;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position + transform.forward * _interactOffset;
        Gizmos.DrawWireSphere(origin, _interactRange);
        Vector3 leftDir = Quaternion.Euler(0, -_interactAngle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, _interactAngle, 0) * transform.forward;
        Gizmos.DrawRay(origin, leftDir * _interactRange);
        Gizmos.DrawRay(origin, rightDir * _interactRange);
        Gizmos.DrawRay(origin, transform.forward * _interactRange);
    }
}