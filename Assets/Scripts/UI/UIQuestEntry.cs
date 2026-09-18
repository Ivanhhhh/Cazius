using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIQuestEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private GameObject _newQuestMark;
    [SerializeField] private GameObject _tooltip;
    [SerializeField] private TMP_Text _tooltipText;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _completedColor = new Color(1f, 0.84f, 0f); // Gold

    [Header("Tooltip Animation")]
    [SerializeField] private float _animDuration = 0.15f;
    //[SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private string _questID;
    private string _questTitleID;
    private string _questTooltipID;
    private bool _seen = false;
    private Coroutine _animCoroutine;

    public void Setup(QuestDefinition quest)
    {
        _questID = quest.questID;
        _questTitleID = quest.questTitle;
        _questTooltipID = quest.questTooltip;

        _seen = false;
        _newQuestMark.SetActive(true);
        _tooltip.SetActive(false);
        _tooltip.transform.localScale = Vector3.zero;

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

        // Only show new quest mark if not yet seen
        _newQuestMark.SetActive(!_seen);
    }

    // --- Hover ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mark as seen and hide new quest mark
        if (!_seen)
        {
            _seen = true;
            _newQuestMark.SetActive(false);
        }

        // Show and animate tooltip
        _tooltipText.text = LocalizationManager.Instance.GetTranslate(_questTooltipID);
        _tooltip.SetActive(true);

        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _animCoroutine = StartCoroutine(AnimateTooltip(Vector3.zero, Vector3.one));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _tooltip.SetActive(false);
        _tooltip.transform.localScale = Vector3.zero;
    }

    // --- Tooltip scale animation ---

    private IEnumerator AnimateTooltip(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < _animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / _animDuration);
            _tooltip.transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        _tooltip.transform.localScale = to;
    }
}
