using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestKillCondition",
                 menuName = "Quests/Conditions/Quest Kill Condition")]
public class QuestKillCondition : QuestCondition
{
    public override bool IsMet(string targetID)
    {
        return QuestManager.Instance.WasKilled(targetID);
    }
}
