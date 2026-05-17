using UnityEngine;

public class PurgatoryObjectToggle : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        if (WorldChangeManager.Instance == null)
            return;

        WorldChangeManager.Instance.SwapToEdenEvent -= DisableObject;
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= EnableObject;
    }

    private System.Collections.IEnumerator SubscribeWhenReady()
    {
        while (WorldChangeManager.Instance == null)
        {
            yield return null;
        }

        WorldChangeManager.Instance.SwapToEdenEvent += DisableObject;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += EnableObject;
    }

    private void DisableObject()
    {
        if (objectToToggle != null)
            objectToToggle.SetActive(false);
    }

    private void EnableObject()
    {
        if (objectToToggle != null)
            objectToToggle.SetActive(true);
    }
}