using UnityEngine;
using UnityEngine.UI;


public class ButtonsBehaviours : MonoBehaviour
{
    [SerializeField] PauseInputHandler _PauseInputHandler;

    [SerializeField] Canvas _OptionsMenu;

    [SerializeField] Button _ResumeButton;

    [SerializeField] Button _OptionsButton;

    [SerializeField] Button _QuitGameButton;



    void Start()
    {
        _ResumeButton.onClick.AddListener(ResumeGame);
        _OptionsButton.onClick.AddListener(OptionsMenu);
        _QuitGameButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        //_ResumeButton.onClick.AddListener(ResumeGame);
        //_OptionsButton.onClick.AddListener(OptionsMenu);
        //_QuitGameButton.onClick.AddListener(QuitGame);
    }

    public void ResumeGame()
    {
        _PauseInputHandler.OnPause(default);
    }

    public void OptionsMenu()
    {
        _OptionsMenu.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
