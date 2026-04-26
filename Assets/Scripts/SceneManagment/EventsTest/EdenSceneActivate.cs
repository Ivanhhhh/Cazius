using UnityEngine;

public class EdenSceneActivate : MonoBehaviour
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
        scene.SetActive(true);
    }
    private void SwapToPurgatory()
    {
        scene.SetActive(false);
    }
}
