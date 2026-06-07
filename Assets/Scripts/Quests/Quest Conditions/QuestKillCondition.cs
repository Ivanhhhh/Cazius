using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestKillCondition", 
                 menuName = "Quests/Conditions/Quest Kill Condition")]
public class QuestKillCondition : QuestCondition
{
    [Tooltip("Must match the enemyID on the QuestEnemy component")]
    public string enemyID;

    public override bool IsMet()
    {
        return QuestManager.Instance.WasKilled(enemyID);
    }
}
