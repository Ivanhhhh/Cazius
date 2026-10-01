using UnityEngine;

public abstract class QuestCondition : ScriptableObject
{
    public abstract bool IsMet(string targetID);

    // Called when the stage owning this condition becomes active
    // Override in conditions that need to subscribe to events
    public virtual void OnQuestActivated() { }
}