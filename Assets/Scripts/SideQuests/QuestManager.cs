using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<string, QuestStatus> _questStates = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Called by QuestGiverNPC on first interact — does not overwrite loaded state
    public void RegisterQuest(string questID)
    {
        if (!_questStates.ContainsKey(questID))
            _questStates[questID] = QuestStatus.NotStarted;
    }

    public QuestStatus GetStatus(string questID)
    {
        return _questStates.TryGetValue(questID, out var status)
            ? status
            : QuestStatus.NotStarted;
    }

    public void StartQuest(string questID)
    {
        if (!_questStates.ContainsKey(questID)) return;
        if (_questStates[questID] != QuestStatus.NotStarted) return;

        _questStates[questID] = QuestStatus.Active;
        SaveManager.Instance.Save();
    }

    public void CompleteQuest(string questID)
    {
        if (!_questStates.ContainsKey(questID)) return;
        if (_questStates[questID] != QuestStatus.Active) return;

        _questStates[questID] = QuestStatus.Completed;
        SaveManager.Instance.Save();
    }

    // --- Save / Load integration ---

    public Dictionary<string, QuestStatus> GetSaveData()
    {
        return new Dictionary<string, QuestStatus>(_questStates);
    }

    public void LoadSaveData(Dictionary<string, QuestStatus> saved)
    {
        _questStates = new Dictionary<string, QuestStatus>(saved);
    }
}
