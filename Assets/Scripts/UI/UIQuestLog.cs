using System.Collections.Generic;
using UnityEngine;

public class UIQuestLog : MonoBehaviour
{
    [SerializeField] private GameObject _questEntryPrefab;

    [Header("Main Quests")]
    [SerializeField] private Transform _mainQuestContainer;

    [Header("Side Quests")]
    [SerializeField] private Transform _sideQuestContainer;

    private readonly Dictionary<string, UIQuestEntry> _entries = new();

    private void OnEnable()
    {
        QuestManager.OnQuestUpdated += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        QuestManager.OnQuestUpdated -= Refresh;
    }

    private void Refresh()
    {
        // Update existing entries
        foreach (var entry in _entries.Values)
            entry.Refresh();

        // Spawn entries for newly discovered quests
        foreach (var quest in QuestManager.Instance.GetAllQuests())
        {
            if (_entries.ContainsKey(quest.questID)) continue;

            Transform container = quest.questType == QuestType.Main
                ? _mainQuestContainer
                : _sideQuestContainer;

            GameObject go = Instantiate(_questEntryPrefab, container);
            UIQuestEntry entry = go.GetComponent<UIQuestEntry>();
            entry.Setup(quest);
            _entries[quest.questID] = entry;
        }
    }
}
