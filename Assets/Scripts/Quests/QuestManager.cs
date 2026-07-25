using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<string, QuestDefinition> _questDefinitions = new();
    private Dictionary<string, QuestStatus> _questStates = new();
    private HashSet<string> _killedEnemies = new();
    private HashSet<string> _reachedLocations = new();

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

    // --- Quest registration ---

    public void RegisterQuest(QuestDefinition quest)
    {
        if (_questStates.ContainsKey(quest.questID)) return;
        _questDefinitions[quest.questID] = quest;
        _questStates[quest.questID] = QuestStatus.NotStarted;
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
        _questStates[questID] = QuestStatus.JustCompleted;
        SaveManager.Instance.Save();
    }

    public void AcknowledgeCompletion(string questID)
    {
        if (!_questStates.ContainsKey(questID)) return;
        if (_questStates[questID] != QuestStatus.JustCompleted) return;
        _questStates[questID] = QuestStatus.Completed;
        SaveManager.Instance.Save();
    }

    // --- UI queries ---

    public List<QuestDefinition> GetQuestsByType(QuestType type)
    {
        return _questDefinitions.Values
            .Where(q => q.questType == type)
            .ToList();
    }

    public List<QuestDefinition> GetAllQuests()
    {
        return _questDefinitions.Values.ToList();
    }

    // --- Kill tracking ---

    public void RegisterKill(string enemyID)
    {
        _killedEnemies.Add(enemyID);
    }

    public bool WasKilled(string enemyID)
    {
        return _killedEnemies.Contains(enemyID);
    }

    // --- Location tracking ---

    public void RegisterLocation(string locationID)
    {
        _reachedLocations.Add(locationID);
    }

    public bool WasReached(string locationID)
    {
        return _reachedLocations.Contains(locationID);
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
