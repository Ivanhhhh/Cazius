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
        _QuitGameButton.onClick.AddListener(ShowConfirmationPopup);
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
    public void ShowConfirmationPopup()
    {
        if(ConfirmationPopup.Instance  != null)
        {
            ConfirmationPopup.Instance.Show
                (
                yesAction: QuitGame,
                message: "Are you sure you want to quit?",
                noAction:null,
                yes:"Yes",
                no:"no"
                );
        }
        else
        {
            Debug.LogError("ConfirmationPopup Not Found");
            QuitGame();
        }
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
