using System;
using System.Linq;
using UnityEngine;

public class NPCQuestGiver : MonoBehaviour, IEInteractable
{
    [Header("Quest")]
    [SerializeField] private QuestDefinition quest;

    [Header("Interaction")]
    [SerializeField] private string _interactText = "F";
    [SerializeField] private Transform _interactionUIPoint;

    [Header("Reward")]
    [SerializeField] private ItemData _questPrizeItem;

    [Header("Item Quest")]
    [SerializeField] private bool _removeItemOnCompletion = false;
    [SerializeField] private string _defaultItemTurnInConfirmID = "dialog_default_item_turn_in_confirm";

    /*
    [Header("Map")]
    [SerializeField] private bool _showOnMap = false;
    [SerializeField] private Vector3 _questDestination;
    public bool ShowOnMap => _showOnMap;
    public Vector3 QuestDestination => _questDestination;
    */

    // --- Add-on seams ---

    // Triggered at the start of every interaction
    public event Action<Transform> OnInteracted;

    // Triggered when a conversation with this NPC ends
    public event Action OnDialogClosed;

    // Triggered once, at the fade midpoint, after the quest is first completed.
    // The fade only runs if something subscribes to this event.
    public event Action OnCompletionApplied;

    // --- Public API for add-ons and other systems ---

    public QuestDefinition Quest => quest;

    public QuestStatus CurrentStatus =>
        quest != null && QuestManager.Instance != null
            ? QuestManager.Instance.GetStatus(quest.questID)
            : QuestStatus.NotStarted;

    public bool IsQuestCompleted =>
        CurrentStatus == QuestStatus.Completed || CurrentStatus == QuestStatus.JustCompleted;


    // --- IEInteractable ---

    public string GetInteractText() => _interactText;
    public Transform GetTransform() => transform;
    public bool IsLocked() => false;

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null ? _interactionUIPoint : transform;
    }

    public void Interact(Transform interactorTransform)
    {
        OnInteracted?.Invoke(interactorTransform);

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
                if (quest.condition != null && quest.condition.IsMet(quest.conditionTargetID))
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

    // --- Localization helper ---

    private string[] Translate(string[] ids)
    {
        if (ids == null || ids.Length == 0) return new string[0];
        return ids.Select(id => LocalizationManager.Instance.GetTranslate(id)).ToArray();
    }

    private void HandleDialogClosed()
    {
        OnDialogClosed?.Invoke();
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

                    if (QuestManager.Instance.GetStatus(quest.questID) == QuestStatus.Active)
                        OpenStageOfferDialog();
                    else
                        HandleDialogClosed();
                },
                onClose: HandleDialogClosed
            );
            return;
        }

        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.offerDialog),
            onAccept: () =>
            {
                QuestManager.Instance.StartQuest(quest.questID);
                HandleDialogClosed();
            },
            onClose: HandleDialogClosed
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
            onClose: HandleDialogClosed
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
                        onClose: HandleDialogClosed
                    );
                    return;
                }
            }
        }

        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.activeDialog),
            onAccept: null,
            onClose: HandleDialogClosed
        );
    }

    private void OpenCompletionDialog()
    {
        if (quest.useStages)
        {
            int stageIndex = QuestManager.Instance.GetStageIndex(quest.questID);
            QuestStage currentStage = (quest.stages != null && stageIndex >= 0 && stageIndex < quest.stages.Count)
                ? quest.stages[stageIndex]
                : null;

            bool isItemTurnInStage = currentStage != null
                                  && currentStage.removeItemOnAdvance
                                  && currentStage.itemToRemoveOnAdvance != null;

            if (isItemTurnInStage)
            {
                OpenItemTurnInConfirmation(
                    customDialog: currentStage.itemTurnInConfirmDialog,
                    onAcceptTurnIn: () => ProcessMultiStageAdvancement(currentStage)
                );
                return;
            }

            ProcessMultiStageAdvancement(currentStage);
            return;
        }

        if (_removeItemOnCompletion)
        {
            OpenItemTurnInConfirmation(
                customDialog: quest.itemTurnInConfirmDialog,
                onAcceptTurnIn: () => ProcessSingleStageCompletion()
            );
            return;
        }

        ProcessSingleStageCompletion();
    }

    private void OpenItemTurnInConfirmation(string[] customDialog, Action onAcceptTurnIn)
    {
        string[] dialogPages;

        if (customDialog != null && customDialog.Length > 0)
        {
            dialogPages = Translate(customDialog);
        }
        else
        {
            string fallbackTranslated = LocalizationManager.Instance.GetTranslate(_defaultItemTurnInConfirmID);
            dialogPages = new string[] { fallbackTranslated };
        }

        DialogUIController.Instance.OpenDialog(
            pages: dialogPages,
            onAccept: onAcceptTurnIn,
            onClose: HandleDialogClosed
        );
    }

    private void ProcessMultiStageAdvancement(QuestStage currentStage)
    {
        bool didAdvance = QuestManager.Instance.TryAdvanceStage(quest.questID);
        if (!didAdvance)
        {
            HandleDialogClosed();
            return;
        }

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
                onClose: () => OpenStageOfferDialog()
            );
            return;
        }

        OpenStageOfferDialog();
    }

    private void ProcessSingleStageCompletion()
    {
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
                // JustCompleted - Completed happens once, so this is the only time the effects run
                bool isFirstClose = QuestManager.Instance.GetStatus(quest.questID) == QuestStatus.JustCompleted;

                QuestManager.Instance.AcknowledgeCompletion(quest.questID);
                HandleDialogClosed();

                if (isFirstClose)
                    ApplyCompletionEffects();
            }
        );
    }

    private void ApplyCompletionEffects()
    {
        // Nothing subscribed means no add-on has anything to change, so skip the fade
        if (OnCompletionApplied == null) return;

        if (UIFadeInFadeOut.Instance != null)
            UIFadeInFadeOut.Instance.FadeOutIn(() => OnCompletionApplied?.Invoke());
        else
            OnCompletionApplied.Invoke();
    }

    private void OpenCompletedDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.completedDialog),
            onAccept: null,
            onClose: HandleDialogClosed
        );
    }

}
