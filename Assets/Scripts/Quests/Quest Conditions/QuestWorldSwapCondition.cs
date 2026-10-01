using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestWorldSwapCondition",
                 menuName = "Quests/Conditions/Quest World Swap Condition")]
public class QuestWorldSwapCondition : QuestCondition
{
    [SerializeField] private bool triggerOnEdenSwap = true;
    [SerializeField] private bool triggerOnPurgatorySwap;

    private bool _triggeredForEden;
    private bool _triggeredForPurgatory;
    private bool _subscribed;

    private void OnEnable()
    {
        _subscribed = false;
        _triggeredForEden = false;
        _triggeredForPurgatory = false;
    }

    public override void OnQuestActivated()
    {
        if (_subscribed) return;
        if (WorldChangeManager.Instance == null) return;

        WorldChangeManager.Instance.SwapToEdenEvent += HandleEdenSwap;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += HandlePurgatorySwap;
        _subscribed = true;

        // Reset flags when stage activates so previous swaps don't count
        _triggeredForEden = false;
        _triggeredForPurgatory = false;
    }

    private void HandleEdenSwap()
    {
        if (triggerOnEdenSwap)
            _triggeredForEden = true;
    }

    private void HandlePurgatorySwap()
    {
        if (triggerOnPurgatorySwap)
            _triggeredForPurgatory = true;
    }

    public override bool IsMet(string targetID)
    {
        if (!_subscribed) return false;

        if (string.Equals(targetID, "Eden", System.StringComparison.OrdinalIgnoreCase))
            return _triggeredForEden;

        if (string.Equals(targetID, "Purgatory", System.StringComparison.OrdinalIgnoreCase))
            return _triggeredForPurgatory;

        return _triggeredForEden || _triggeredForPurgatory;
    }
}
