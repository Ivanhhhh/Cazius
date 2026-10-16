using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIQuestNotification : MonoBehaviour
{
    public static UIQuestNotification Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TMP_Text _questTypeText;
    [SerializeField] private TMP_Text _questTitleText;
    [SerializeField] private TMP_Text _completeText;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelRect;

    [Header("Slide Settings")]
    [SerializeField] private float _slideDuration = 0.4f;
    [SerializeField] private float _holdDuration = 2.5f;
    [SerializeField] private float _slideOutDuration = 0.4f;
    [SerializeField] private float _leftMargin = 20f;

    [Header("Flicker Settings")]
    [SerializeField] private int _flickerCount = 4;
    [SerializeField] private float _flickerSpeed = 0.07f;
    [SerializeField] private SFXManager.SFXCategoryType _flickerSFX = SFXManager.SFXCategoryType.HoverButton;

    [Header("Localization IDs")]
    [SerializeField] private string _mainQuestLabelID = "notification_main_quest";
    [SerializeField] private string _sideQuestLabelID = "notification_side_quest";
    [SerializeField] private string _completeLabelID = "notification_complete";

    private Queue<(string questID, bool isComplete)> _queue = new();
    private bool _isPlaying = false;
    private float _panelWidth;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _panelWidth = _panelRect.rect.width;
        _canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        QuestManager.OnQuestStarted += OnQuestStarted;
        QuestManager.OnQuestJustCompleted += OnQuestJustCompleted;
    }

    private void OnDisable()
    {
        QuestManager.OnQuestStarted -= OnQuestStarted;
        QuestManager.OnQuestJustCompleted -= OnQuestJustCompleted;
    }

    private void OnQuestStarted(string questID)
    {
        _queue.Enqueue((questID, false));
        if (!_isPlaying) StartCoroutine(PlayQueue());
    }

    private void OnQuestJustCompleted(string questID)
    {
        _queue.Enqueue((questID, true));
        if (!_isPlaying) StartCoroutine(PlayQueue());
    }

    private IEnumerator PlayQueue()
    {
        _isPlaying = true;

        while (_queue.Count > 0)
        {
            var (questID, isComplete) = _queue.Dequeue();
            yield return StartCoroutine(ShowNotification(questID, isComplete));
        }

        _isPlaying = false;
    }

    private IEnumerator ShowNotification(string questID, bool isComplete)
    {
        var allQuests = QuestManager.Instance.GetAllQuests();
        QuestDefinition quest = allQuests.Find(q => q.questID == questID);
        if (quest == null) yield break;

        // Set text
        _questTypeText.text = quest.questType == QuestType.Main
            ? LocalizationManager.Instance.GetTranslate(_mainQuestLabelID)
            : LocalizationManager.Instance.GetTranslate(_sideQuestLabelID);

        _questTitleText.text = LocalizationManager.Instance.GetTranslate(quest.questTitle);
        _completeText.text = LocalizationManager.Instance.GetTranslate(_completeLabelID);
        _completeText.gameObject.SetActive(isComplete);

        float startX = -_panelWidth - _leftMargin;
        float endX = _leftMargin;

        // Reset
        _canvasGroup.alpha = 1f;
        _panelRect.anchoredPosition = new Vector2(startX, _panelRect.anchoredPosition.y);

        // --- Slide in ---
        float elapsed = 0f;
        while (elapsed < _slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _slideDuration);
            _panelRect.anchoredPosition = new Vector2(Mathf.Lerp(startX, endX, t), _panelRect.anchoredPosition.y);
            yield return null;
        }
        _panelRect.anchoredPosition = new Vector2(endX, _panelRect.anchoredPosition.y);

        // --- Flicker using alpha ---
        for (int i = 0; i < _flickerCount; i++)
        {
            _canvasGroup.alpha = 0f;
            yield return new WaitForSecondsRealtime(_flickerSpeed);
            _canvasGroup.alpha = 1f;
            SFXManager.Instance.PlaySFX(_flickerSFX);
            yield return new WaitForSecondsRealtime(_flickerSpeed);
        }

        // --- Hold ---
        yield return new WaitForSecondsRealtime(_holdDuration);

        // --- Slide out ---
        elapsed = 0f;
        Vector2 currentPos = _panelRect.anchoredPosition;
        while (elapsed < _slideOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _slideOutDuration);
            _panelRect.anchoredPosition = new Vector2(Mathf.Lerp(endX, startX, t), _panelRect.anchoredPosition.y);
            yield return null;
        }

        _panelRect.anchoredPosition = new Vector2(startX, _panelRect.anchoredPosition.y);
        _canvasGroup.alpha = 0f;
    }

}
