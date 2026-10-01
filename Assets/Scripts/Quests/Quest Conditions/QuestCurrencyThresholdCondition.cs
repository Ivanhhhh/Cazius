using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestCurrencyThresholdCondition",
                 menuName = "Quests/Conditions/Quest Currency Threshold Condition")]
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
