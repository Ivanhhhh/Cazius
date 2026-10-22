using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NPCQuestGiver))]
public class NPCFacePlayer : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 5f;

    [Header("When to rotate")]
    [SerializeField] private bool _rotateTowardsPlayer = true;
    [Tooltip("Untick for NPCs that get repositioned or animated after the quest")]
    [SerializeField] private bool _rotateTowardsPlayerAfterCompletion = true;

    private NPCQuestGiver _questGiver;
    private Coroutine _rotateCoroutine;

    private void Awake()
    {
        _questGiver = GetComponent<NPCQuestGiver>();
    }

    private void OnEnable()
    {
        _questGiver.OnInteracted += HandleInteracted;
    }

    private void OnDisable()
    {
        _questGiver.OnInteracted -= HandleInteracted;
        StopRotation();
    }

    // Called by NPCOnCompletionBehavior right before it moves the NPC
    public void StopRotation()
    {
        if (_rotateCoroutine == null) return;

        StopCoroutine(_rotateCoroutine);
        _rotateCoroutine = null;
    }

    private void HandleInteracted(Transform interactor)
    {
        bool isCompleted = _questGiver.CurrentStatus == QuestStatus.Completed;
        bool canRotate = isCompleted ? _rotateTowardsPlayerAfterCompletion : _rotateTowardsPlayer;
        if (!canRotate) return;

        Vector3 dir = interactor.position - transform.position;
        dir.y = 0;
        if (dir == Vector3.zero) return;

        StopRotation();
        _rotateCoroutine = StartCoroutine(RotateCoroutine(Quaternion.LookRotation(dir)));
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
        _rotateCoroutine = null;
    }

}
