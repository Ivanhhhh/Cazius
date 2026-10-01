using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    public static event Action OnQuestUpdated;

    private Dictionary<string, QuestDefinition> _questDefinitions = new();
    private Dictionary<string, QuestStatus> _questStates = new();
    private Dictionary<string, int> _questStageIndexes = new();
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
        if (quest == null) return;
        if (_questStates.ContainsKey(quest.questID)) return;

        _questDefinitions[quest.questID] = quest;
        _questStates[quest.questID] = QuestStatus.NotStarted;
        _questStageIndexes[quest.questID] = 0;
        OnQuestUpdated?.Invoke();
    }

    public QuestStatus GetStatus(string questID)
    {
        return _questStates.TryGetValue(questID, out var status)
            ? status
            : QuestStatus.NotStarted;
    }

    public int GetStageIndex(string questID)
    {
        return _questStageIndexes.TryGetValue(questID, out var index)
            ? index
            : 0;
    }

    public bool IsCurrentStageConditionMet(string questID)
    {
        if (!_questDefinitions.TryGetValue(questID, out var quest) || quest == null)
            return false;

        if (!quest.useStages || quest.stages == null || quest.stages.Count == 0)
            return quest.condition != null && quest.condition.IsMet(quest.conditionTargetID);

        int stageIndex = GetStageIndex(questID);
        if (stageIndex < 0 || stageIndex >= quest.stages.Count)
            return false;

        QuestStage stage = quest.stages[stageIndex];
        return stage != null && stage.condition != null && stage.condition.IsMet(stage.targetID);
    }

    public bool TryAdvanceStage(string questID)
    {
        if (!_questDefinitions.TryGetValue(questID, out var quest) || quest == null)
            return false;

        if (!quest.useStages || quest.stages == null || quest.stages.Count == 0)
        {
            if (quest.condition == null || !quest.condition.IsMet(quest.conditionTargetID))
                return false;

            CompleteQuest(questID);
            return true;
        }

        int stageIndex = GetStageIndex(questID);
        if (stageIndex < 0 || stageIndex >= quest.stages.Count)
            return false;

        QuestStage stage = quest.stages[stageIndex];
        if (stage == null || stage.condition == null || !stage.condition.IsMet(stage.targetID))
            return false;

        if (stage.rewardItem != null)
            Inventory.Instance.AddItem(stage.rewardItem);

        if (stage.removeRewardItemOnAdvance && !string.IsNullOrEmpty(stage.targetID))
            Inventory.Instance.RemoveItem(stage.targetID);

        stageIndex++;
        _questStageIndexes[questID] = stageIndex;

        if (stageIndex >= quest.stages.Count)
        {
            _questStates[questID] = QuestStatus.JustCompleted;
            if (SaveManager.Instance != null)
                SaveManager.Instance.Save();
            OnQuestUpdated?.Invoke();
            return true;
        }

        if (_questStates[questID] == QuestStatus.NotStarted)
            _questStates[questID] = QuestStatus.Active;
        else if (_questStates[questID] != QuestStatus.Active)
            _questStates[questID] = QuestStatus.Active;

        if (SaveManager.Instance != null)
            SaveManager.Instance.Save();
        OnQuestUpdated?.Invoke();
        return true;
    }

    public void StartQuest(string questID)
    {
        if (!_questStates.ContainsKey(questID)) return;
        if (_questStates[questID] != QuestStatus.NotStarted) return;
        _questStates[questID] = QuestStatus.Active;
        if (SaveManager.Instance != null)
            SaveManager.Instance.Save();
        OnQuestUpdated?.Invoke();
    }

    public void CompleteQuest(string questID)
    {
        if (!_questStates.ContainsKey(questID)) return;
        if (_questStates[questID] != QuestStatus.Active) return;
        _questStates[questID] = QuestStatus.JustCompleted;
        if (SaveManager.Instance != null)
            SaveManager.Instance.Save();
        OnQuestUpdated?.Invoke();
    }

    public void AcknowledgeCompletion(string questID)
    {
        if (!_questStates.ContainsKey(questID)) return;
        if (_questStates[questID] != QuestStatus.JustCompleted) return;
        _questStates[questID] = QuestStatus.Completed;
        if (SaveManager.Instance != null)
            SaveManager.Instance.Save();
        OnQuestUpdated?.Invoke();
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

    public Dictionary<string, int> GetStageSaveData()
    {
        return new Dictionary<string, int>(_questStageIndexes);
    }

    public void LoadSaveData(Dictionary<string, QuestStatus> saved)
    {
        _questStates = new Dictionary<string, QuestStatus>(saved);
        foreach (var questID in _questDefinitions.Keys)
        {
            if (!_questStageIndexes.ContainsKey(questID))
                _questStageIndexes[questID] = 0;
        }
    }

    public void LoadSaveData(Dictionary<string, QuestStatus> saved, Dictionary<string, int> stageIndexes)
    {
        _questStates = new Dictionary<string, QuestStatus>(saved);
        _questStageIndexes = stageIndexes != null
            ? new Dictionary<string, int>(stageIndexes)
            : new Dictionary<string, int>();

        foreach (var questID in _questDefinitions.Keys)
        {
            if (!_questStageIndexes.ContainsKey(questID))
                _questStageIndexes[questID] = 0;
        }
    }
}
