using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private GameObject _soundConfigPanel;

    [Header("Main Panel")]
    [SerializeField] private string _startGameScene = "LoadToGameFromMenu";
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;

    [Header("Options Panel")]
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _soundConfigButton;
    [SerializeField] private Button _optionsBackButton;

    [Header("Controls Panel")]
    [SerializeField] private Button _controlsBackButton;

    [Header("Sound Config Panel")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Button _soundConfigBackButton;

    private GameObject _currentPanel;

    private void Start()
    {
        // Register button listeners
        _startGameButton.onClick.AddListener(OnStartGame);
        _optionsButton.onClick.AddListener(OnOpenOptions);
        _exitButton.onClick.AddListener(OnExit);

        _controlsButton.onClick.AddListener(OnOpenControls);
        _soundConfigButton.onClick.AddListener(OnOpenSoundConfig);
        _optionsBackButton.onClick.AddListener(OnOptionsBack);

        _controlsBackButton.onClick.AddListener(OnControlsBack);

        _masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        _soundConfigBackButton.onClick.AddListener(OnSoundConfigBack);

        // Start on the main panel
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

    // Main Panel Handlers
    private void OnStartGame()
    {
        SceneManager.LoadScene(_startGameScene);
    }

    private void OnOpenOptions() => ShowPanel(_optionsPanel);

    private void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Options Panel Handlers
    private void OnOpenControls() => ShowPanel(_controlsPanel);
    private void OnOpenSoundConfig() => ShowPanel(_soundConfigPanel);
    private void OnOptionsBack() => ShowPanel(_mainPanel);

    // Controls Panel Handlers
    private void OnControlsBack() => ShowPanel(_optionsPanel);

    // Sound Config Panel Handlers
    private void OnMasterVolumeChanged(float value)
    {
        // TODO: connect to AudioManager
    }

    private void OnMusicVolumeChanged(float value)
    {
        // TODO: connect to AudioManager
    }

    private void OnSFXVolumeChanged(float value)
    {
        // TODO: connect to AudioManager
    }

    private void OnSoundConfigBack() => ShowPanel(_optionsPanel);
}