using UnityEngine;

public class NPCItemQuestGiver : NPCQuestGiverBase
{
    protected override bool IsConditionMet()
    {
        if (quest.condition is QuestItemCondition hasItemCondition)
            return hasItemCondition.IsMet();

        Debug.LogWarning($"[NPCItemQuestGiver] Expected QuestItemCondition on quest '{quest.questID}'");
        return false;
    }

    protected override void OnQuestCompleted()
    {
        // Remove the required item from inventory on completion
        if (quest.condition is QuestItemCondition hasItemCondition)
            Inventory.Instance.RemoveItem(hasItemCondition.itemID);
    }
}