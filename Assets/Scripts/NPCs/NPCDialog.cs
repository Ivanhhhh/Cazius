using System.Linq;
using UnityEngine;
using System.Collections;

public class NPCDialog : MonoBehaviour, IEInteractable
{
    [Header("Dialog")]
    [SerializeField] private string[] _dialogPages;

    [Header("NPC Behavior")]
    [SerializeField] private string _interactText = "F";
    [SerializeField] private string _wavingTrigger = "Waving";
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private SFXManager.SFXCategoryType _interactSFX = SFXManager.SFXCategoryType.MaleHeySFX;

    [Header("Idle Flavor")]
    [SerializeField] private int _idleVariant = 0;

    [Header("Interaction")]
    [SerializeField] private bool _rotateTowardsPlayer = true;

    [SerializeField] private Transform _interactionUIPoint;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    private Animator _animator;
    private static readonly int _idleVariantHash = Animator.StringToHash("IdleVariant");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        Debug.Log($"Controller assigned: {_animator.runtimeAnimatorController?.name}");
        foreach (var p in _animator.parameters)
        {
            Debug.Log($"Param name: '{p.name}', type: {p.type}, hash: {p.nameHash}");
        }

        _animator.SetInteger(_idleVariantHash, _idleVariant);
        _animator.Update(0f);
        Debug.Log($"{name}: OnEnable set IdleVariant={_idleVariant}, animator now reads {_animator.GetInteger(_idleVariantHash)}");
    }

    public void Interact(Transform interactorTransform)
    {
        if (_rotateTowardsPlayer)
            RotateTowardsPlayer(interactorTransform);

        _animator.SetTrigger(_wavingTrigger);
        SFXManager.Instance.PlaySFXAtPosition(_interactSFX, transform.position);

        DialogUIController.Instance.OpenDialog(
            pages: Translate(_dialogPages),
            onAccept: null,
            onClose: null
        );
    }

    public string GetInteractText() { return _interactText; }
    public Transform GetTransform() { return transform; }

    // --- Localization ---
    private string[] Translate(string[] ids)
    {
        return ids.Select(id => LocalizationManager.Instance.GetTranslate(id)).ToArray();
    }

    // --- Helpers ---
    private void RotateTowardsPlayer(Transform interactorTransform)
    {
        Vector3 dir = interactorTransform.position - transform.position;
        dir.y = 0;
        if (dir == Vector3.zero) return;
        StartCoroutine(RotateCoroutine(Quaternion.LookRotation(dir)));
    }

    private IEnumerator RotateCoroutine(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    public bool IsLocked() { return false; }

}
