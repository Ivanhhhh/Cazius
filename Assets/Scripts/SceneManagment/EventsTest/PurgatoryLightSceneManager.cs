using UnityEngine;
using UnityEngine.SceneManagement;

public class PurgatoryLightSceneManager : MonoBehaviour
{
    [SerializeField] private string _lightingSceneName;

    private void OnEnable()
    {
        WorldChangeManager.Instance.SwapToPurgatoryEvent += SwapToPurgatory;
    }
    private void OnDisable()
    {
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= SwapToPurgatory;
    }

    private void SwapToPurgatory()
    {
        Scene lightingScene = SceneManager.GetSceneByName(_lightingSceneName);

        if (lightingScene.IsValid() && lightingScene.isLoaded)
        {
            SceneManager.SetActiveScene(lightingScene);
        }
    }
}
