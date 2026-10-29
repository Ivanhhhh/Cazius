using UnityEngine;
using UnityEngine.SceneManagement;

public class EdenLightSceneManager : MonoBehaviour
{

    [SerializeField] private string _lightingSceneName;
    private void OnEnable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent += SwapToEden;
    }
    private void OnDisable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent -= SwapToEden;
    }

    private void SwapToEden()
    {
        Scene lightingScene = SceneManager.GetSceneByName(_lightingSceneName);

        if (lightingScene.IsValid() && lightingScene.isLoaded)
        {
            SceneManager.SetActiveScene(lightingScene);
        }
    }
}
