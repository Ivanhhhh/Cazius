using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestWorldSwapCondition",
                 menuName = "Quests/Conditions/Quest World Swap Condition")]
public class QuestWorldSwapCondition : QuestCondition
{
    [SerializeField] private bool triggerOnEdenSwap = true;
    [SerializeField] private bool triggerOnPurgatorySwap;

    private bool _triggeredForEden;
    private bool _triggeredForPurgatory;

    private void EnsureSubscribed()
    {
        if (WorldChangeManager.Instance == null)
            return;

        WorldChangeManager.Instance.SwapToEdenEvent -= HandleEdenSwap;
        WorldChangeManager.Instance.SwapToEdenEvent += HandleEdenSwap;

        WorldChangeManager.Instance.SwapToPurgatoryEvent -= HandlePurgatorySwap;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += HandlePurgatorySwap;
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
        EnsureSubscribed();

        if (string.Equals(targetID, "Eden", System.StringComparison.OrdinalIgnoreCase))
            return _triggeredForEden || (WorldChangeManager.Instance != null && WorldChangeManager.Instance.IsInEden && triggerOnEdenSwap);

        if (string.Equals(targetID, "Purgatory", System.StringComparison.OrdinalIgnoreCase))
            return _triggeredForPurgatory || (WorldChangeManager.Instance != null && !WorldChangeManager.Instance.IsInEden && triggerOnPurgatorySwap);

        return _triggeredForEden || _triggeredForPurgatory;
    }
}
