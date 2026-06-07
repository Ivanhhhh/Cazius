using UnityEngine;

public class NPCKillQuestGiver : NPCQuestGiverBase
{
    protected override bool IsConditionMet()
    {
        if (quest.condition is QuestKillCondition killCondition)
            return killCondition.IsMet();

        Debug.LogWarning($"[NPCKillQuestGiver] Expected QuestKillCondition on quest '{quest.questID}'");
        return false;
    }
}