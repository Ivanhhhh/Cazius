using System.Collections.Generic;
using UnityEngine;

public enum QuestType { Main, Side }

[System.Serializable]
public class QuestStage
{
    [Header("Condition")]
    public QuestCondition condition;
    public string targetID;

    [Header("Reward")]
    public ItemData rewardItem;
    public bool removeRewardItemOnAdvance;

    [Header("Flow")]
    public bool isFinalStage;

    [Header("Stage dialog")]
    [TextArea(2, 5)]
    public string[] stageReadyDialog;
}

[CreateAssetMenu(fileName = "NewQuest",
                 menuName = "Quests/Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identity")]
    public string questID;
    public string questTitle;
    public string questTooltip;
    public QuestType questType;

    [Header("Legacy single-stage condition")]
    public QuestCondition condition;
    public string conditionTargetID;

    [Header("Multi-stage quest")]
    public bool useStages;
    public List<QuestStage> stages = new List<QuestStage>();

    [Header("Dialog � Offer")]
    [TextArea(2, 5)]
    public string[] offerDialog;

    [Header("Dialog � Active (condition not yet met)")]
    [TextArea(2, 5)]
    public string[] activeDialog;

    [Header("Dialog � First Completion (plays once)")]
    [TextArea(2, 5)]
    public string[] firstCompletionDialog;

    [Header("Dialog � Completed (plays every time after)")]
    [TextArea(2, 5)]
    public string[] completedDialog;
}
