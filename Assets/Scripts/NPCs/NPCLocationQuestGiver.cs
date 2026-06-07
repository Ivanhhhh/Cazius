using UnityEngine;

public class NPCLocationQuestGiver : NPCQuestGiverBase
{
    protected override bool IsConditionMet()
    {
        if (quest.condition is QuestLocationCondition locationCondition)
            return locationCondition.IsMet();

        Debug.LogWarning($"[NPCLocationQuestGiver] Expected QuestLocationCondition on quest '{quest.questID}'");
        return false;
    }
}