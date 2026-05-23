using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    [SerializeField] string SceneName;

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneName);
    }
}
