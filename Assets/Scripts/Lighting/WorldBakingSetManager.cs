
using UnityEngine;
using UnityEngine.Rendering;

public class WorldBakingSetManager : MonoBehaviour
{
    [SerializeField] private ProbeVolumeBakingSet _edenBakingSet;
    [SerializeField] private ProbeVolumeBakingSet _purgatoryBakingSet;

    private WorldChangeManager _worldChangeManager;

    private void Start()
    {
        _worldChangeManager = WorldChangeManager.Instance;

        if (_worldChangeManager == null)
        {
            return;
        }

        _worldChangeManager.SwapToEdenEvent += SetEdenBakingSet;
        _worldChangeManager.SwapToPurgatoryEvent += SetPurgatoryBakingSet;

        SetBakingSet(_worldChangeManager.IsInEden
            ? _edenBakingSet
            : _purgatoryBakingSet);
    }

    private void OnDestroy()
    {
        if (_worldChangeManager == null) return;

        _worldChangeManager.SwapToEdenEvent -= SetEdenBakingSet;
        _worldChangeManager.SwapToPurgatoryEvent -= SetPurgatoryBakingSet;
    }

    private void SetEdenBakingSet()
    {
        SetBakingSet(_edenBakingSet);
    }

    private void SetPurgatoryBakingSet()
    {
        SetBakingSet(_purgatoryBakingSet);
    }

    private void SetBakingSet(ProbeVolumeBakingSet bakingSet)
    {
        if (bakingSet == null) return;

        var probeVolume = ProbeReferenceVolume.instance;

        if (probeVolume == null || !probeVolume.isInitialized)
        {
            return;
        }

        if (probeVolume.currentBakingSet == bakingSet)
            return;

        probeVolume.SetActiveBakingSet(bakingSet);
    }
}
