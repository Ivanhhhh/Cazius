using System.Linq;
using UnityEngine;

public class NPCDialog : MonoBehaviour, IEInteractable
{
    [Header("Dialog")]
    [SerializeField] private string[] _dialogPages;

    [Header("NPC Behavior")]
    [SerializeField] private string _interactText = "F to talk";
    [SerializeField] private string _wavingTrigger = "Waving";
    [SerializeField] private SFXManager.SFXCategoryType _interactSFX = SFXManager.SFXCategoryType.MaleHeySFX;

    [SerializeField] private Transform _interactionUIPoint;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact(Transform interactorTransform)
    {
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

    // --- Localization helper ---

    private string[] Translate(string[] ids)
    {
        return ids.Select(id => LocalizationManager.Instance.GetTranslate(id)).ToArray();
    }

    // --- Helpers ---

    private void RotateTowardsPlayer(Transform interactorTransform)
    {
        Vector3 dir = interactorTransform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}