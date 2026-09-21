using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIQuestEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private GameObject _newQuestMark;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _completedColor = new Color(1f, 0.84f, 0f);

    private string _questID;
    private string _questTitleID;
    private string _questTooltipID;
    private bool _seen = false;

    public void Setup(QuestDefinition quest)
    {
        _questID = quest.questID;
        _questTitleID = quest.questTitle;
        _questTooltipID = quest.questTooltip;

        _seen = false;
        _newQuestMark.SetActive(true);

        Refresh();
    }

    public void Refresh()
    {
        QuestStatus status = QuestManager.Instance.GetStatus(_questID);

        _titleText.text = LocalizationManager.Instance.GetTranslate(_questTitleID);

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
        _newQuestMark.SetActive(!_seen);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_seen)
        {
            _seen = true;
            _newQuestMark.SetActive(false);
        }

        string tooltipText = LocalizationManager.Instance.GetTranslate(_questTooltipID);
        UITooltip.Instance.Show(tooltipText, GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITooltip.Instance.Hide();
    }

}
