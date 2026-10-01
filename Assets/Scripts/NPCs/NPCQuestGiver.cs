using System.Collections;
using System.Linq;
using UnityEngine;

public class NPCQuestGiver : MonoBehaviour, IEInteractable
{
    [Header("Quest")]
    [SerializeField] private QuestDefinition quest;

    [Header("NPC Behavior")]
    [SerializeField] private string _interactText = "F to talk";
    [SerializeField] private string _wavingTrigger = "Waving";
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private SFXManager.SFXCategoryType _interactSFX = SFXManager.SFXCategoryType.MaleHeySFX;

    [Header("Reward")]
    [SerializeField] private ItemData _questPrizeItem;

    [Header("Item Quest")]
    [SerializeField] private bool _removeItemOnCompletion = false;

    [Header("Map")]
    [SerializeField] private bool _showOnMap = false;
    [SerializeField] private Vector3 _questDestination;

    [Header("Reposition After Quest")]
    [SerializeField] private bool _moveAfterCompletion = false;
    [SerializeField] private Vector3 _newPosition;
    [SerializeField] private Vector3 _newRotationEuler;

    private Animator _animator;

    [SerializeField] private Transform _interactionUIPoint;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact(Transform interactorTransform)
    {
        RotateTowardsPlayer(interactorTransform);
        _animator.SetTrigger(_wavingTrigger);
        SFXManager.Instance.PlaySFXAtPosition(_interactSFX, transform.position);

        QuestManager.Instance.RegisterQuest(quest);
        QuestStatus status = QuestManager.Instance.GetStatus(quest.questID);

        if (quest.useStages)
        {
            switch (status)
            {
                case QuestStatus.NotStarted:
                    OpenOfferDialog();
                    break;

                case QuestStatus.Active:
                    if (QuestManager.Instance.IsCurrentStageConditionMet(quest.questID))
                        OpenCompletionDialog();
                    else
                        OpenActiveDialog();
                    break;

                case QuestStatus.JustCompleted:
                    OpenFirstCompletionDialog();
                    break;

                case QuestStatus.Completed:
                    OpenCompletedDialog();
                    break;
            }
            return;
        }

        switch (status)
        {
            case QuestStatus.NotStarted:
                OpenOfferDialog();
                break;

            case QuestStatus.Active:
                if (quest.condition.IsMet(quest.conditionTargetID))
                    OpenCompletionDialog();
                else
                    OpenActiveDialog();
                break;

            case QuestStatus.JustCompleted:
                OpenFirstCompletionDialog();
                break;

            case QuestStatus.Completed:
                OpenCompletedDialog();
                break;
        }
    }

    public bool ShowOnMap => _showOnMap;
    public Vector3 QuestDestination => _questDestination;

    public string GetInteractText() { return _interactText; }
    public Transform GetTransform() { return transform; }

    public bool IsLocked() { return false; }

    // --- Localization helper ---

    private string[] Translate(string[] ids)
    {
        return ids.Select(id => LocalizationManager.Instance.GetTranslate(id)).ToArray();
    }

    // --- Dialog openers ---

    private void OpenOfferDialog()
    {
        if (quest.useStages)
        {
            DialogUIController.Instance.OpenDialog(
                pages: Translate(quest.offerDialog),
                onAccept: () =>
                {
                    QuestManager.Instance.StartQuest(quest.questID);
                    OpenStageOfferDialog();
                },
                onClose: null
            );
            return;
        }

        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.offerDialog),
            onAccept: () => QuestManager.Instance.StartQuest(quest.questID),
            onClose: null
        );
    }

    private void OpenStageOfferDialog()
    {
        if (!quest.useStages) return;

        int stageIndex = QuestManager.Instance.GetStageIndex(quest.questID);
        if (quest.stages == null || stageIndex < 0 || stageIndex >= quest.stages.Count)
        {
            OpenActiveDialog();
            return;
        }

        QuestStage stage = quest.stages[stageIndex];
        if (stage == null || stage.stageOfferDialog == null || stage.stageOfferDialog.Length == 0)
        {
            OpenActiveDialog();
            return;
        }

        DialogUIController.Instance.OpenDialog(
            pages: Translate(stage.stageOfferDialog),
            onAccept: null,
            onClose: () => OpenActiveDialog()
        );
    }

    private void OpenActiveDialog()
    {
        if (quest.useStages)
        {
            int stageIndex = QuestManager.Instance.GetStageIndex(quest.questID);
            if (quest.stages != null && stageIndex >= 0 && stageIndex < quest.stages.Count)
            {
                QuestStage stage = quest.stages[stageIndex];
                if (stage != null && stage.stageActiveDialog != null && stage.stageActiveDialog.Length > 0)
                {
                    DialogUIController.Instance.OpenDialog(
                        pages: Translate(stage.stageActiveDialog),
                        onAccept: null,
                        onClose: null
                    );
                    return;
                }
            }
        }

        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.activeDialog),
            onAccept: null,
            onClose: null
        );
    }

    private void OpenCompletionDialog()
    {
        if (quest.useStages)
        {
            int stageIndex = QuestManager.Instance.GetStageIndex(quest.questID);
            QuestStage currentStage = quest.stages != null && stageIndex >= 0 && stageIndex < quest.stages.Count
                ? quest.stages[stageIndex]
                : null;

            bool didAdvance = QuestManager.Instance.TryAdvanceStage(quest.questID);
            if (!didAdvance) return;

            bool isNowJustCompleted = QuestManager.Instance.GetStatus(quest.questID) == QuestStatus.JustCompleted;

            if (currentStage != null && currentStage.stageReadyDialog != null && currentStage.stageReadyDialog.Length > 0)
            {
                DialogUIController.Instance.OpenDialog(
                    pages: Translate(currentStage.stageReadyDialog),
                    onAccept: null,
                    onClose: () =>
                    {
                        if (QuestManager.Instance.GetStatus(quest.questID) == QuestStatus.JustCompleted)
                        {
                            if (_questPrizeItem != null)
                                Inventory.Instance.AddItem(_questPrizeItem);
                            OpenFirstCompletionDialog();
                        }
                        else
                            OpenStageOfferDialog();
                    }
                );
                return;
            }

            if (isNowJustCompleted)
            {
                if (_questPrizeItem != null)
                    Inventory.Instance.AddItem(_questPrizeItem);
                OpenFirstCompletionDialog();
            }
            else
                OpenStageOfferDialog();

            return;
        }

        QuestManager.Instance.CompleteQuest(quest.questID);

        if (_questPrizeItem != null)
            Inventory.Instance.AddItem(_questPrizeItem);

        if (_removeItemOnCompletion)
            Inventory.Instance.RemoveItem(quest.conditionTargetID);

        OpenFirstCompletionDialog();
    }

    private void OpenFirstCompletionDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.firstCompletionDialog),
            onAccept: null,
            onClose: () =>
            {
                QuestManager.Instance.AcknowledgeCompletion(quest.questID);

                if (_moveAfterCompletion)
                {
                    transform.position = _newPosition;
                    transform.rotation = Quaternion.Euler(_newRotationEuler);
                }
            }
        );
    }

    private void OpenCompletedDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.completedDialog),
            onAccept: null,
            onClose: null
        );
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

}
