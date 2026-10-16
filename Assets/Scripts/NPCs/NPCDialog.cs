using System.Collections;
using System.Linq;
using UnityEngine;

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
    private NPCController _npcController;
    private static readonly int _idleVariantHash = Animator.StringToHash("IdleVariant");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        _npcController = GetComponent<NPCController>();
    }

    private void OnEnable()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (_animator != null)
        {
            _animator.SetInteger(_idleVariantHash, _idleVariant);
            _animator.Update(0f);
        }
    }

    public void Interact(Transform interactorTransform)
    {
        if (_npcController != null && _npcController.enabled && _npcController.Machine != null)
        {
            _npcController.Machine.ChangeState(_npcController.TalkingState);
        }

        if (_rotateTowardsPlayer)
            RotateTowardsPlayer(interactorTransform);

        if (_animator != null && !string.IsNullOrEmpty(_wavingTrigger))
            _animator.SetTrigger(_wavingTrigger);

        SFXManager.Instance.PlaySFXAtPosition(_interactSFX, transform.position);

        DialogUIController.Instance.OpenDialog(
            pages: Translate(_dialogPages),
            onAccept: null,
            onClose: OnDialogClose
        );
    }

    private void OnDialogClose()
    {
        if (_npcController != null && _npcController.enabled && _npcController.Machine != null)
        {
            _npcController.Machine.ChangeState(_npcController.IdleState);
        }
    }

    public string GetInteractText() { return _interactText; }
    public Transform GetTransform() { return transform; }

    // --- Localization ---
    private string[] Translate(string[] ids)
    {
        if (ids == null || ids.Length == 0) return new string[0];
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