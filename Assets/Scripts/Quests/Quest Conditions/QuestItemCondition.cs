using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestItemCondition",
                 menuName = "Quests/Conditions/Quest Item Condition")]
public class QuestItemCondition : QuestCondition
{
    public override bool IsMet(string targetID)
    {
        return Inventory.Instance.HasItem(targetID);
    }
}
