using UnityEngine;

public abstract class QuestCondition : ScriptableObject
{
    public abstract bool IsMet(string targetID);
}
