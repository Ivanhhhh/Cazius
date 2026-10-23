using UnityEngine;

[RequireComponent(typeof(NPCQuestGiver))]
public class NPCInteractFeedback : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private string _wavingTrigger = "Waving";

    [Header("Sound")]
    [SerializeField] private SFXManager.SFXCategoryType _interactSFX = SFXManager.SFXCategoryType.MaleHeySFX;

    private NPCQuestGiver _questGiver;
    private Animator _animator;

    private void Awake()
    {
        _questGiver = GetComponent<NPCQuestGiver>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        _questGiver.OnInteracted += HandleInteracted;
    }

    private void OnDisable()
    {
        _questGiver.OnInteracted -= HandleInteracted;
    }

    private void HandleInteracted(Transform interactor)
    {
        if (_animator != null && !string.IsNullOrEmpty(_wavingTrigger))
            _animator.SetTrigger(_wavingTrigger);

        if (SFXManager.Instance != null)
            SFXManager.Instance.PlaySFXAtPosition(_interactSFX, transform.position);
    }

}
