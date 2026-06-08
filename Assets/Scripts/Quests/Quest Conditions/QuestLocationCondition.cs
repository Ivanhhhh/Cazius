using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestLocationCondition",
                 menuName = "Quests/Conditions/Quest Location Condition")]
public class QuestLocationCondition : QuestCondition
{
    public override bool IsMet(string targetID)
    {
        return QuestManager.Instance.WasReached(targetID);
    }
}
