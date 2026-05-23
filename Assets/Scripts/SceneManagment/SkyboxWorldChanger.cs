using System.Collections;
using UnityEngine;

public class SkyboxWorldChanger : MonoBehaviour
{
    [SerializeField] private Material edenSkybox;
    [SerializeField] private Material purgatorySkybox;

    [SerializeField] private bool updateGlobalIllumination = true;

    private IEnumerator Start()
    {
        while (WorldChangeManager.Instance == null)
        {
            yield return null;
        }

        WorldChangeManager.Instance.SwapToEdenEvent += SetEdenSkybox;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += SetPurgatorySkybox;
    }

    private void OnDestroy()
    {
        if (WorldChangeManager.Instance == null)
            return;

        WorldChangeManager.Instance.SwapToEdenEvent -= SetEdenSkybox;
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= SetPurgatorySkybox;
    }

    private void SetEdenSkybox()
    {
        ChangeSkybox(edenSkybox);
    }

    private void SetPurgatorySkybox()
    {
        ChangeSkybox(purgatorySkybox);
    }

    private void ChangeSkybox(Material newSkybox)
    {
        if (newSkybox == null)
        {
            return;
        }

        RenderSettings.skybox = newSkybox;

        if (updateGlobalIllumination)
        {
            DynamicGI.UpdateEnvironment();
        }
    }
}