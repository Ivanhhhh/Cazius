using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class UIMainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _RebindPanel;
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _optionsPanel;

    [Header("Sub Panels")]
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private GameObject _soundPanel;
    [SerializeField] private GameObject _languagePanel;

    [Header("Main Panel - Buttons")]
    [SerializeField] private string _startGameScene = "LoadToGameFromMenu";
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _RebindButton;

    [Header("Options Panel - Buttons")]
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _soundConfigButton;
    [SerializeField] private Button _languageButton;
    [SerializeField] private Button _backToMainButton;

    private GameObject _currentPanel;
    private GameObject _currentSubPanel;

    private bool TogglePanelRebind = false;

    private byte OpenedAmount = 0;

    private void Start()
    {
        // Main panel listeners
        _startGameButton.onClick.AddListener(OnStartGame);
        _optionsButton.onClick.AddListener(OnOpenOptions);
        _exitButton.onClick.AddListener(OnExit);

        // Options panel listeners
        _controlsButton.onClick.AddListener(OnOpenControls);
        _soundConfigButton.onClick.AddListener(OnOpenSoundConfig);
        _languageButton.onClick.AddListener(OnOpenLanguage);
        _backToMainButton.onClick.AddListener(OnBackToMain);
        _RebindButton.onClick.AddListener(RebindPanelMethod);


        // Start on main panel
        ShowPanel(_mainPanel);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OpenedAmount = 2;
           _RebindPanel.SetActive(false);
            OpenedAmount = 0;

        }
    }

    // Panel Navigation
    private void ShowPanel(GameObject panel)
    {
        if (_currentPanel != null)
            _currentPanel.SetActive(false);

        _currentPanel = panel;
        _currentPanel.SetActive(true);
    }

    // Sub Panel Navigation (Controls/Sound inside Options)
    private void ShowSubPanel(GameObject subPanel)
    {
        if (_currentSubPanel != null)
            _currentSubPanel.SetActive(false);

        _currentSubPanel = subPanel;
        _currentSubPanel.SetActive(true);
    }

    // Main Panel Handlers
    private void OnStartGame()
    {
        SceneManager.LoadScene(_startGameScene);
        MusicManager.Instance.PlayTrack(MusicManager.MusicTrack.Eden1);
    }

    private void OnOpenOptions()
    {
        ShowPanel(_optionsPanel);
        ShowSubPanel(_controlsPanel); // Controls open by default
    }

    private void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Options Panel Handlers
    private void OnOpenControls() => ShowSubPanel(_controlsPanel);
    private void OnOpenSoundConfig() => ShowSubPanel(_soundPanel);
    private void OnOpenLanguage() => ShowSubPanel(_languagePanel);
    private void OnBackToMain() => ShowPanel(_mainPanel);

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
        if (TogglePanelRebind) _RebindPanel.SetActive(true);

        else if (TogglePanelRebind != true) _RebindPanel.SetActive(false);
        print ("Ejecutado");
    }

}
