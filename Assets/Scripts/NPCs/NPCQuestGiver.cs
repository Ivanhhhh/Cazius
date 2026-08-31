using System.Linq;
using UnityEngine;

public class NPCQuestGiver : MonoBehaviour, IEInteractable
{
    [Header("Quest")]
    [SerializeField] private QuestDefinition quest;

    [Header("NPC Behavior")]
    [SerializeField] private string _interactText = "F to talk";
    [SerializeField] private string _wavingTrigger = "Waving";

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
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.MaleHeySFX, transform.position);

        QuestManager.Instance.RegisterQuest(quest);
        QuestStatus status = QuestManager.Instance.GetStatus(quest.questID);

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

    // --- Localization helper ---

    private string[] Translate(string[] ids)
    {
        return ids.Select(id => LocalizationManager.Instance.GetTranslate(id)).ToArray();
    }

    // --- Dialog openers ---

    private void OpenOfferDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.offerDialog),
            onAccept: () => QuestManager.Instance.StartQuest(quest.questID),
            onClose: null
        );
    }

    private void OpenActiveDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: Translate(quest.activeDialog),
            onAccept: null,
            onClose: null
        );
    }

    private void OpenCompletionDialog()
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
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
