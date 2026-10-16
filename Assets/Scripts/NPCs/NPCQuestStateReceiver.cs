using UnityEngine;

public class NPCQuestStateReceiver : MonoBehaviour
{
    [Header("Quest Target")]
    [SerializeField] private QuestDefinition _quest;

    [Header("Reposition & Animation After Quest")]
    [SerializeField] private bool _moveOnCompletion = true;
    [SerializeField] private Vector3 _newPosition;
    [SerializeField] private Vector3 _newRotationEuler;

    [SerializeField] private bool _changeAnimationOnCompletion = false;
    [SerializeField] private AnimationType _completionAnimType = AnimationType.Trigger;
    [SerializeField] private string _animParamName = "IdleVariant";
    [SerializeField] private int _newAnimIntValue = 1;

    [Header("Patrol After Quest (Optional)")]
    [SerializeField] private bool _enablePatrolOnCompletion = false;
    [SerializeField] private NPCController _npcController;

    private Animator _animator;
    private bool _hasBeenRepositioned = false;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        if (_npcController == null)
            _npcController = GetComponent<NPCController>();
    }

    private void OnEnable()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        // Subscribe to static event
        QuestManager.OnQuestJustCompleted += HandleQuestJustCompleted;

        ApplyStateFromQuestStatus();
    }

    private void OnDisable()
    {
        // Unsubscribe from static event to prevent dangling references
        QuestManager.OnQuestJustCompleted -= HandleQuestJustCompleted;
    }

    private void Start()
    {
        ApplyStateFromQuestStatus();
    }

    private void HandleQuestJustCompleted(string completedQuestID)
    {
        if (_quest != null && _quest.questID == completedQuestID)
        {
            ApplyStateFromQuestStatus();
        }
    }

    public void ApplyStateFromQuestStatus()
    {
        if (_quest == null || QuestManager.Instance == null) return;

        QuestStatus status = QuestManager.Instance.GetStatus(_quest.questID);

        if (status == QuestStatus.Completed || status == QuestStatus.JustCompleted || _hasBeenRepositioned)
        {
            _hasBeenRepositioned = true;

            if (_moveOnCompletion)
            {
                transform.localPosition = _newPosition;
                transform.localRotation = Quaternion.Euler(_newRotationEuler);
            }

            if (_changeAnimationOnCompletion && _animator != null && !string.IsNullOrEmpty(_animParamName))
            {
                switch (_completionAnimType)
                {
                    case AnimationType.Trigger:
                        _animator.SetTrigger(_animParamName);
                        break;

                    case AnimationType.Int:
                        _animator.SetInteger(_animParamName, _newAnimIntValue);
                        break;
                }
            }

            if (_enablePatrolOnCompletion && _npcController != null)
            {
                _npcController.enabled = true;
            }
        }
    }

}
