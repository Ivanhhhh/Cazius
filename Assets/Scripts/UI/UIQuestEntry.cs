using TMPro;
using UnityEngine;

public class UIQuestEntry : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _statusText;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _completedColor = new Color(1f, 0.84f, 0f); // Gold

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

        _statusText.color = isCompleted ? _completedColor : _defaultColor;
    }
}
