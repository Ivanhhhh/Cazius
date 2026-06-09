using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestKeyItemCondition", 
                 menuName = "Quests/Conditions/Quest Key Item Condition")]
public class QuestKeyItemCondition : QuestCondition
{
    public override bool IsMet(string targetID) => targetID switch
    {
        "EdenKey" => KeyInventorySystem.Instance.HasEdenKey,
        "PurgatoryKey" => KeyInventorySystem.Instance.HasPurgatoryKey,
        _ => false
    };
}
