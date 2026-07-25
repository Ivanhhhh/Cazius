using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestEntry : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Settings")]
    [SerializeField] private float _completedAlpha = 0.4f;

    private string _questID;

    public void Setup(QuestDefinition quest)
    {
        _questID = quest.questID;
        _titleText.text = quest.questTitle;
        Refresh();
    }

    public void Refresh()
    {
        QuestStatus status = QuestManager.Instance.GetStatus(_questID);

        bool isCompleted = status == QuestStatus.JustCompleted
                        || status == QuestStatus.Completed;

        _statusText.text = status switch
        {
            QuestStatus.NotStarted => "Not Accepted",
            QuestStatus.Active => "Active",
            QuestStatus.JustCompleted => "Completed",
            QuestStatus.Completed => "Completed",
            _ => string.Empty
        };

        _canvasGroup.alpha = isCompleted ? _completedAlpha : 1f;
    }
}
