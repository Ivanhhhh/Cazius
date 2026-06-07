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

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact(Transform interactorTransform)
    {
        RotateTowardsPlayer(interactorTransform);
        _animator.SetTrigger(_wavingTrigger);
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.MaleHeySFX, transform.position);

        QuestManager.Instance.RegisterQuest(quest.questID);
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

            case QuestStatus.Completed:
                OpenCompletedDialog();
                break;
        }
    }

    public string GetInteractText() { return _interactText; }
    public Transform GetTransform() { return transform; }

    // --- Dialog openers ---

    private void OpenOfferDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: quest.offerDialog,
            onAccept: () => QuestManager.Instance.StartQuest(quest.questID),
            onClose: null
        );
    }

    private void OpenActiveDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: quest.activeDialog,
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

        DialogUIController.Instance.OpenDialog(
            pages: quest.completedDialog,
            onAccept: null,
            onClose: null
        );
    }

    private void OpenCompletedDialog()
    {
        DialogUIController.Instance.OpenDialog(
            pages: quest.completedDialog,
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
