using UnityEngine;

public class NPCQuestGiver : MonoBehaviour
{

    [Header("Quest")]
    [SerializeField] private QuestDefinition quest;

    [Header("References")]
    [SerializeField] private DialogUIController dialogUI;

    public void Interact()
    {
        QuestManager.Instance.RegisterQuest(quest.questID);

        QuestStatus status = QuestManager.Instance.GetStatus(quest.questID);

        switch (status)
        {
            case QuestStatus.NotStarted:
                OpenOfferDialog();
                break;

            case QuestStatus.Active:
                if (Inventory.Instance.HasItem(quest.requiredItemID))
                    OpenCompletionDialog();
                else
                    OpenActiveDialog();
                break;

            case QuestStatus.Completed:
                OpenCompletedDialog();
                break;
        }
    }

    // --- Dialog openers ---

    private void OpenOfferDialog()
    {
        dialogUI.OpenDialog(
            pages: quest.offerDialog,
            onAccept: () => QuestManager.Instance.StartQuest(quest.questID),
            onClose: null
        );
    }

    private void OpenActiveDialog()
    {
        dialogUI.OpenDialog(
            pages: quest.activeDialog,
            onAccept: null,
            onClose: null
        );
    }

    private void OpenCompletionDialog()
    {
        QuestManager.Instance.CompleteQuest(quest.questID);

        dialogUI.OpenDialog(
            pages: quest.completedDialog,
            onAccept: null,
            onClose: null
        );
    }

    private void OpenCompletedDialog()
    {
        dialogUI.OpenDialog(
            pages: quest.completedDialog,
            onAccept: null,
            onClose: null
        );
    }
}
