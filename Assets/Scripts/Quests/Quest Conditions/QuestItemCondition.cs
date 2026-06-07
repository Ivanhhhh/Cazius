using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestItemCondition", 
                 menuName = "Quests/Conditions/Quest Item Condition")]
public class QuestItemCondition : QuestCondition
{
    [Tooltip("Must match the itemID on the ItemData asset")]
    public string itemID;

    public override bool IsMet()
    {
        return Inventory.Instance.HasItem(itemID);
    }
}
