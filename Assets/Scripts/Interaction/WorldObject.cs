using UnityEngine;

public class WorldObject : MonoBehaviour // Va en cada objecto que se quiera prender/desactivar por escena 
{
    [Header("Worlds")]
    [SerializeField] private bool _showInEden;
    [SerializeField] private bool _showInPurgatory;

    [SerializeField] private GameObject _visualRoot;
    [SerializeField] private Collider _collider;

    void Start()
    {
        WorldChangeManager.Instance.SwapToEdenEvent += HandleSwapToEden;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += HandleSwapToPurgatory;
        UpdateWorldVisibility();
        _collider = GetComponent<Collider>();
        if (_visualRoot == null) Debug.Log("Tenemos q setear la visual root");
    }
    /*
    void OnEnable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent += HandleSwapToEden;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += HandleSwapToPurgatory;
    }

    void OnDisable()
    {
        if (WorldChangeManager.Instance == null) return;

        WorldChangeManager.Instance.SwapToEdenEvent -= HandleSwapToEden;
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= HandleSwapToPurgatory;
    }*/

    private void HandleSwapToEden()
    {
        _visualRoot.SetActive(_showInEden);
        if (_collider) _collider.enabled = _showInEden;
    }

    private void HandleSwapToPurgatory()
    {
        if (_visualRoot == null) return;
        _visualRoot.SetActive(_showInPurgatory);

        if (_collider) _collider.enabled = _showInPurgatory;
    }

    private void UpdateWorldVisibility()
    {
        bool isInEden = WorldChangeManager.Instance.IsInEden;

        if (isInEden)
        {
            _visualRoot.SetActive(_showInEden);
            _collider.enabled = _showInEden;
        }
        else
        {
            _visualRoot.SetActive(_showInPurgatory);
            _collider.enabled = _showInPurgatory;
        }
    }
}
