using UnityEngine;

public enum AnimationType
{
    Trigger,
    Int
}

[RequireComponent(typeof(NPCQuestGiver))]
public class NPCOnCompletionBehavior : MonoBehaviour
{
    [Header("Reposition")]
    [SerializeField] private bool _moveAfterCompletion = false;
    [SerializeField] private Vector3 _newPosition;
    [SerializeField] private Vector3 _newRotationEuler;

    [Header("Animation")]
    [SerializeField] private bool _changeAnimation = false;
    [SerializeField] private AnimationType _animType = AnimationType.Int;
    [SerializeField] private string _animParamName = "IdleVariant";
    [Tooltip("Int only. Applied while the quest is not completed")]
    [SerializeField] private int _initialIntValue = 0;
    [Tooltip("Int only. Applied once the quest is completed")]
    [SerializeField] private int _afterIntValue = 1;

    [Header("Patrol")]
    [Tooltip("Keeps NPCController disabled until the quest is completed")]
    [SerializeField] private bool _enablePatrol = false;

    private NPCQuestGiver _questGiver;
    private Animator _animator;
    private NPCController _npcController;
    private NPCFacePlayer _facePlayer;
    private bool _stateApplied;

    private void Awake()
    {
        _questGiver = GetComponent<NPCQuestGiver>();
        _animator = GetComponentInChildren<Animator>();
        _npcController = GetComponent<NPCController>();
        TryGetComponent(out _facePlayer);
    }

    private void OnEnable()
    {
        _questGiver.OnCompletionApplied += HandleCompletionApplied;
        _stateApplied = TryApplyStateFromQuestStatus();
    }

    private void Start()
    {
        // QuestManager lives in the Persistent scene and may not exist yet during OnEnable
        if (!_stateApplied)
            _stateApplied = TryApplyStateFromQuestStatus();
    }

    private void OnDisable()
    {
        _questGiver.OnCompletionApplied -= HandleCompletionApplied;
    }

    // --- State derived from quest status ---

    private bool TryApplyStateFromQuestStatus()
    {
        if (QuestManager.Instance == null || _questGiver.Quest == null)
            return false;

        if (_questGiver.IsQuestCompleted)
            ApplyAfterState();
        else
            ApplyBeforeState();

        return true;
    }

    private void ApplyBeforeState()
    {
        if (_enablePatrol && _npcController != null)
            _npcController.enabled = false;

        if (CanUseAnimator() && _animType == AnimationType.Int)
        {
            _animator.SetInteger(_animParamName, _initialIntValue);
            _animator.Update(0f);
        }
    }

    private void ApplyAfterState()
    {
        if (_enablePatrol && _npcController != null)
            _npcController.enabled = true;

        if (!CanUseAnimator()) return;

        switch (_animType)
        {
            case AnimationType.Trigger:
                _animator.SetTrigger(_animParamName);
                break;

            case AnimationType.Int:
                _animator.SetInteger(_animParamName, _afterIntValue);
                break;
        }
    }

    private bool CanUseAnimator()
    {
        return _changeAnimation && _animator != null && !string.IsNullOrEmpty(_animParamName);
    }

    // --- Fade midpoint, raised by NPCQuestGiver ---

    private void HandleCompletionApplied()
    {
        // A turn towards the player still in progress would fight the reposition
        if (_facePlayer != null)
            _facePlayer.StopRotation();

        if (_moveAfterCompletion)
        {
            transform.localPosition = _newPosition;
            transform.localRotation = Quaternion.Euler(_newRotationEuler);
        }

        ApplyAfterState();
    }

}
