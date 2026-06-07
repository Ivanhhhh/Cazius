using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestLocationCondition",
                 menuName = "Quests/Conditions/Quest Location Condition")]
public class QuestLocationCondition : QuestCondition
{
    [Tooltip("Must match the locationID on the QuestLocationTrigger component")]
    public string locationID;

    public override bool IsMet()
    {
        return QuestManager.Instance.WasReached(locationID);
    }
}
