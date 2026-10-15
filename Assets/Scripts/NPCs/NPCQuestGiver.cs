using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public enum AnimationType
{
    Trigger,
    Int
}

public class NPCQuestGiver : MonoBehaviour, IEInteractable
{
    [Header("Quest")]
    [SerializeField] private QuestDefinition quest;

    [Header("NPC Behavior")]
    [SerializeField] private string _interactText = "F to talk";
    [SerializeField] private string _wavingTrigger = "Waving";
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private bool _rotateTowardsPlayer = true;
    [SerializeField] private SFXManager.SFXCategoryType _interactSFX = SFXManager.SFXCategoryType.MaleHeySFX;

    [Header("Idle Variant (Initial)")]
    [SerializeField] private int _idleVariant = 0;
    [SerializeField] private string _idleVariantParamName = "IdleVariant";

    [Header("Reward")]
    [SerializeField] private ItemData _questPrizeItem;

    [Header("Item Quest")]
    [SerializeField] private bool _removeItemOnCompletion = false;
    [SerializeField] private string _defaultItemTurnInConfirmID = "dialog_default_item_turn_in_confirm";

    [Header("Map")]
    [SerializeField] private bool _showOnMap = false;
    [SerializeField] private Vector3 _questDestination;

    [Header("Reposition & Animation After Quest")]
    [SerializeField] private bool _moveAfterCompletion = false;
    [SerializeField] private Vector3 _newPosition;
    [SerializeField] private Vector3 _newRotationEuler;

    [SerializeField] private bool _rotateTowardsPlayerAfterCompletion = false;

    [SerializeField] private bool _changeAnimationOnCompletion = false;
    [SerializeField] private AnimationType _completionAnimType = AnimationType.Trigger;
    [SerializeField] private string _animParamName = "IdleVariant";
    [SerializeField] private int _newAnimIntValue = 1;

    private Animator _animator;
    private Coroutine _rotateCoroutine;
    private bool _hasBeenRepositioned = false;

    [SerializeField] private Transform _interactionUIPoint;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null ? _interactionUIPoint : transform;
    }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (_animator != null && !string.IsNullOrEmpty(_idleVariantParamName))
        {
            _animator.SetInteger(_idleVariantParamName, _idleVariant);
            _animator.Update(0f); // Forces Animator to refresh state immediately on load
        }
    }

    public void Interact(Transform interactorTransform)
    {
        bool canRotate = _hasBeenRepositioned ? _rotateTowardsPlayerAfterCompletion : _rotateTowardsPlayer;
        if (canRotate)
            RotateTowardsPlayer(interactorTransform);

        if (_animator != null && !string.IsNullOrEmpty(_wavingTrigger))
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

    public bool ShowOnMap => _showOnMap;
    public Vector3 QuestDestination => _questDestination;

    public string GetInteractText() => _interactText;
    public Transform GetTransform() => transform;

    public bool IsLocked() => false;

    // --- Localization helper ---

    private string[] Translate(string[] ids)
    {
        if (ids == null || ids.Length == 0) return new string[0];
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
                    if (QuestManager.Instance.GetStatus(quest.questID) == QuestStatus.Active)
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
            onClose: null
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
            QuestStage currentStage = (quest.stages != null && stageIndex >= 0 && stageIndex < quest.stages.Count)
                ? quest.stages[stageIndex]
                : null;

            bool isItemTurnInStage = currentStage != null && currentStage.removeItemOnAdvance && currentStage.itemToRemoveOnAdvance != null;

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
            onClose: null
        );
    }

    private void ProcessMultiStageAdvancement(QuestStage currentStage)
    {
        bool didAdvance = QuestManager.Instance.TryAdvanceStage(quest.questID);
        if (!didAdvance) return;

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
                QuestManager.Instance.AcknowledgeCompletion(quest.questID);

                if (!_hasBeenRepositioned && (_moveAfterCompletion || _changeAnimationOnCompletion))
                {
                    _hasBeenRepositioned = true;

                    if (UIFadeInFadeOut.Instance != null)
                    {
                        UIFadeInFadeOut.Instance.FadeOutIn(() =>
                        {
                            ApplyCompletionMoveAndAnimation();
                        });
                    }
                    else
                    {
                        ApplyCompletionMoveAndAnimation();
                    }
                }
            }
        );
    }

    private void ApplyCompletionMoveAndAnimation()
    {
        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }

        if (_moveAfterCompletion)
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

        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);

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
