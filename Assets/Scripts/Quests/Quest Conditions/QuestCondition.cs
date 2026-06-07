using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestCondition", 
                 menuName = "Quests/Quest Condition")]
public abstract class QuestCondition : ScriptableObject
{
    public abstract bool IsMet();
}
