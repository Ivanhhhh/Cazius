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

    [Header("Dialog — Offer")]
    [TextArea(2, 5)]
    public string[] offerDialog;

    [Header("Dialog — Active (condition not yet met)")]
    [TextArea(2, 5)]
    public string[] activeDialog;

    [Header("Dialog — Completed")]
    [TextArea(2, 5)]
    public string[] completedDialog;
}