using UnityEngine;

public class DestroyOnEdenSwap : MonoBehaviour
{
    private void OnDisable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent -= Die;
    }

    private void OnEnable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent += Die;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
