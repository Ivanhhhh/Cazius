using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class ButtonsBehaviours : MonoBehaviour
{
    [SerializeField] GameObject RebindPanel;

    [SerializeField] PauseInputHandler _PauseInputHandler;

    [SerializeField] Canvas _OptionsMenu;

    [SerializeField] Button _ResumeButton;

    [SerializeField] Button _OptionsButton;

    [SerializeField] Button _QuitGameButton;

    [SerializeField] Button _RebindButton;

    private bool TogglePanelRebind = false;

    private byte OpenedAmount = 0;


    void Start()
    {
        _ResumeButton.onClick.AddListener(ResumeGame);
        _OptionsButton.onClick.AddListener(OptionsMenu);
        _QuitGameButton.onClick.AddListener(ShowConfirmationPopup);
        _RebindButton.onClick.AddListener(RebindPanelMethod);

    }

    void Update()
    {
        //_ResumeButton.onClick.AddListener(ResumeGame);
        //_OptionsButton.onClick.AddListener(OptionsMenu);
        //_QuitGameButton.onClick.AddListener(QuitGame);

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OpenedAmount = 2;
           RebindPanel.SetActive(false);
            OpenedAmount = 0;

        }
    }

    public void ResumeGame()
    {
        _PauseInputHandler.OnPause(default);
        OpenedAmount = 0;
        RebindPanel.SetActive(false);
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

    public void RebindPanelMethod()
    {
         Debug.Log(OpenedAmount);
        OpenedAmount += 1;

        if (OpenedAmount <= 1) TogglePanelRebind = true;

        

        if (OpenedAmount >= 2)
        {
            TogglePanelRebind = false;
            OpenedAmount = 0;
        }
        if (TogglePanelRebind) RebindPanel.SetActive(true);

        else if (TogglePanelRebind != true) RebindPanel.SetActive(false);
        print ("Ejecutado");
    }
}
