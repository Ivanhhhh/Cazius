using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest",
                 menuName = "Quests/Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identity")]
    public string questID;
    public string questTitle;

    [Header("Required Condition")]
    public QuestCondition condition;
    public string conditionTargetID;

    [Header("Dialog — Offer")]
    [TextArea(2, 5)]
    public string[] offerDialog;

    [Header("Dialog — Active (condition not yet met)")]
    [TextArea(2, 5)]
    public string[] activeDialog;

    [Header("Dialog — First Completion (plays once)")]
    [TextArea(2, 5)]
    public string[] firstCompletionDialog;

    [Header("Dialog — Completed (plays every time after)")]
    [TextArea(2, 5)]
    public string[] completedDialog;
}