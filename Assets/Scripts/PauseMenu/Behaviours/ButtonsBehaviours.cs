using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ButtonsBehaviours : MonoBehaviour
{
    [SerializeField] GameObject RebindPanel;
    [SerializeField] PauseInputHandler _PauseInputHandler;
    [SerializeField] GameObject SoundConfigPanel;
    [SerializeField] GameObject ConfirmPopPanel;

    [SerializeField] Button _ResumeButton;
    [SerializeField] Button _SoundConfigfButton;
    [SerializeField] Button _QuitGameButton;
    [SerializeField] Button _RebindButton;
    [SerializeField] Button _ConfirmPopButtonYES;
    [SerializeField] Button _ConfirmPopButtonNO;

    void Start()
    {
        _ResumeButton.onClick.AddListener(ResumeGame);
        _SoundConfigfButton.onClick.AddListener(SoundConfig);
        _QuitGameButton.onClick.AddListener(ShowConfirmationPopup);
        _RebindButton.onClick.AddListener(RebindPanelMethod);

        _ConfirmPopButtonYES.onClick.AddListener(QuitGame);
        _ConfirmPopButtonNO.onClick.AddListener(CloseConfirmationPopup);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanels();
        }
    }

    public void ResumeGame()
    {
        _PauseInputHandler.OnPause(default);
        ClosePanels();
    }

    public void SoundConfig()
    {
        RebindPanel.SetActive(false);
        ConfirmPopPanel.SetActive(false);

        SoundConfigPanel.SetActive(!SoundConfigPanel.activeSelf);
    }

    public void RebindPanelMethod()
    {
        SoundConfigPanel.SetActive(false);
        ConfirmPopPanel.SetActive(false);

        RebindPanel.SetActive(!RebindPanel.activeSelf);
    }

    public void ShowConfirmationPopup()
    {
        RebindPanel.SetActive(false);
        SoundConfigPanel.SetActive(false);

        ConfirmPopPanel.SetActive(true);
    }

    public void CloseConfirmationPopup()
    {
        ConfirmPopPanel.SetActive(false);
    }

    private void ClosePanels()
    {
        RebindPanel.SetActive(false);
        SoundConfigPanel.SetActive(false);
        ConfirmPopPanel.SetActive(false);
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

