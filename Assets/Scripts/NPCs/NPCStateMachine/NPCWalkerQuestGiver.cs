using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class NPCWalkerQuestGiver : MonoBehaviour, IEInteractable
{
    [Header("Quest")]
    [SerializeField] private QuestDefinition quest;

    [Header("NPC Behavior")]
    [SerializeField] private string _interactText = "F to talk";
    [SerializeField] private string _wavingTrigger = "Waving";
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

    [SerializeField] private Transform _interactionUIPoint;

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    private NPCController _npc;

    private void Awake()
    {
        _npc = GetComponent<NPCController>();
    }

    public void Interact(Transform interactorTransform)
    {
        _npc.TalkingState.BeginTalking(interactorTransform, _wavingTrigger, _interactSFX);
        _npc.Machine.ChangeState(_npc.TalkingState);

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
    // Every branch returns to Idle on close; Walking resumes on its own from
    // wherever the waypoint progress was left (it lives on NPCController)
    private void OpenOfferDialog()
    {
        if (quest.useStages)
        {
            DialogUIController.Instance.OpenDialog(
                pages: Translate(quest.offerDialog),
                onAccept: () =>
                {
                    QuestManager.Instance.StartQuest(quest.questID);
                    if (QuestManager.Instance.GetStatus(quest.questID) == QuestStatus.Active)
                        OpenStageOfferDialog();
                },
                onClose: () => _npc.Machine.ChangeState(_npc.IdleState)
            );
            return;
        }

        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.offerDialog),
            onAccept: () => QuestManager.Instance.StartQuest(quest.questID),
            onClose: () => _npc.Machine.ChangeState(_npc.IdleState)
        );
    }

    private void OpenStageOfferDialog()
    {
        if (!quest.useStages)
            return;

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
            onClose: () => _npc.Machine.ChangeState(_npc.IdleState)
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
                        onClose: () => _npc.Machine.ChangeState(_npc.IdleState)
                    );
                    return;
                }
            }
        }

        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.activeDialog),
            onAccept: null,
            onClose: () => _npc.Machine.ChangeState(_npc.IdleState)
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
            if (!didAdvance)
                return;

            bool isNowJustCompleted = QuestManager.Instance.GetStatus(quest.questID) == QuestStatus.JustCompleted;

            if (isNowJustCompleted)
            {
                if (_questPrizeItem != null)
                    Inventory.Instance.AddItem(_questPrizeItem);
                OpenFirstCompletionDialog();
                return;
            }

            if (currentStage != null && currentStage.stageReadyDialog != null && currentStage.stageReadyDialog.Length > 0)
            {
                DialogUIController.Instance.OpenDialog(
                    pages: Translate(currentStage.stageReadyDialog),
                    onAccept: null,
                    onClose: () =>
                    {
                        OpenStageOfferDialog();
                        _npc.Machine.ChangeState(_npc.IdleState);
                    }
                );
                return;
            }

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

                _npc.Machine.ChangeState(_npc.IdleState);
            }
        );
    }

    private void OpenCompletedDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.completedDialog),
            onAccept: null,
            onClose: () => _npc.Machine.ChangeState(_npc.IdleState)
        );
    }

}
