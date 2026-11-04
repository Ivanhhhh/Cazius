using System.Collections;
using UnityEngine;

public class NonInteractableDoor : MonoBehaviour, IEInteractable
{
    [Header("References")]
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private Transform _interactionUIPoint;
    [SerializeField] private Collider _interactionCollider;

    [Header("Interaction")]
    [SerializeField] private string _interactText = "F to Open Door";

    [Header("Door Movement")]
    [SerializeField] private float _rotationAngle = 7f;
    [SerializeField] private float _rotateDuration = 0.15f;
    [SerializeField] private float _returnDuration = 0.35f;


    private Quaternion _initialRotation;
    private bool _hasInteracted;

    private void Awake()
    {
        if (_rotationPivot != null)
            _initialRotation = _rotationPivot.localRotation;
    }

    public void Interact(Transform interactorTransform)
    {
        if (_hasInteracted || _rotationPivot == null) return;

        _hasInteracted = true;

        StartCoroutine(AnimateDoor());
    }

    private IEnumerator AnimateDoor()
    {
        Quaternion targetRotation = _initialRotation * Quaternion.Euler(0f, _rotationAngle, 0f);

        yield return RotatePivot(_initialRotation, targetRotation, _rotateDuration);
        yield return RotatePivot(targetRotation, _initialRotation, _returnDuration);

        _rotationPivot.localRotation = _initialRotation;

        if (_interactionCollider != null)
            _interactionCollider.enabled = false;
    }

    private IEnumerator RotatePivot(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            _rotationPivot.localRotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        _rotationPivot.localRotation = to;
    }

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null ? _interactionUIPoint : transform;
    }

    public string GetInteractText() => _interactText;
    public Transform GetTransform() => transform;
    public bool IsLocked() => false;
}