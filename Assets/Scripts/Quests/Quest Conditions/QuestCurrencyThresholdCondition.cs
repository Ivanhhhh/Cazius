using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestSoulEnergyCondition",
                 menuName = "Quests/Conditions/Quest Soul Energy Condition")]
public class QuestSoulEnergyCondition : QuestCondition
{
    public override bool IsMet(string targetID)
    {
        if (SoulEnergyManager.Instance == null)
            return false;

        if (!int.TryParse(targetID, out int requiredAmount))
            return false;

        return SoulEnergyManager.Instance.CurrentSoulEnergy >= requiredAmount;
    }
}
