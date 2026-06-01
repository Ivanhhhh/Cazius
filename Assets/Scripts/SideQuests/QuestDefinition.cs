using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identity")]
    public string questID;
    public string questTitle;

    [Header("Required Condition")]
    public string requiredItemID;

    [Header("Dialog — Offer")]
    [TextArea(2, 5)]
    public string[] offerDialog;

    [Header("Dialog — Active (item not yet found)")]
    [TextArea(2, 5)]
    public string[] activeDialog;

    [Header("Dialog — Completed")]
    [TextArea(2, 5)]
    public string[] completedDialog;
}
