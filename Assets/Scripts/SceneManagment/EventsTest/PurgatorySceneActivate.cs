using UnityEngine;

public class PurgatorySceneActivate : MonoBehaviour
{

    [SerializeField] GameObject scene;

    private void OnEnable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent += SwapToEden;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += SwapToPurgatory;
    }
    private void OnDisable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent -= SwapToEden;
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= SwapToPurgatory;
    }

    private void SwapToEden()
    {
        scene.SetActive(false);
    }
    private void SwapToPurgatory()
    {
        scene.SetActive(true);
    }
}
