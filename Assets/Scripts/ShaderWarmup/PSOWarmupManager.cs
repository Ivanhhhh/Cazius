using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Unity.Jobs;

public class PSOWarmupManager : MonoBehaviour
{
    [Header("PSO Collections")]
    [SerializeField] private GraphicsStateCollection[] collectionsToWarmUp;

    [Header("Progressive Warmup")]
    [SerializeField] private bool useProgressiveWarmup = true;
    [SerializeField] private int statesPerFrame = 64;

    public IEnumerator WarmUp()
    {
        if (collectionsToWarmUp == null || collectionsToWarmUp.Length == 0)
            yield break;

        foreach (GraphicsStateCollection collection in collectionsToWarmUp)
        {
            if (collection == null)
                continue;

            Debug.Log(
                $"Starting PSO warmup: {collection.name}. States: {collection.totalGraphicsStateCount}, API: {collection.graphicsDeviceType}"
            );

            if (!useProgressiveWarmup)
            {
                JobHandle handle = collection.WarmUp();

                // Synchronous warmup. This may freeze briefly, but it happens behind the loading screen.
                handle.Complete();
            }
            else
            {
                int safety = 0;

                while (!collection.isWarmedUp && safety < 10000)
                {
                    JobHandle handle = collection.WarmUpProgressively(statesPerFrame);
                    handle.Complete();

                    safety++;

                    yield return null;
                }
            }

            Debug.Log(
                $"Finished PSO warmup: {collection.name}. Warmed: {collection.completedWarmupCount}/{collection.totalGraphicsStateCount}"
            );

            yield return null;
        }
    }
}