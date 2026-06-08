using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _optionsPanel;

    [Header("Sub Panels")]
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private GameObject _soundPanel;

    [Header("Main Panel - Buttons")]
    [SerializeField] private string _startGameScene = "LoadToGameFromMenu";
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;

    [Header("Options Panel - Buttons")]
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _soundConfigButton;
    [SerializeField] private Button _backToMainButton;

    private GameObject _currentPanel;
    private GameObject _currentSubPanel;

    private void Start()
    {
        // Main panel listeners
        _startGameButton.onClick.AddListener(OnStartGame);
        _optionsButton.onClick.AddListener(OnOpenOptions);
        _exitButton.onClick.AddListener(OnExit);

        // Options panel listeners
        _controlsButton.onClick.AddListener(OnOpenControls);
        _soundConfigButton.onClick.AddListener(OnOpenSoundConfig);
        _backToMainButton.onClick.AddListener(OnBackToMain);

        // Start on main panel
        ShowPanel(_mainPanel);
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
    private void OnBackToMain() => ShowPanel(_mainPanel);

}
