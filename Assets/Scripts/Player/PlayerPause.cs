using UnityEngine;

public class PlayerPause : MonoBehaviour, IPausable
{
    [SerializeField] private MonoBehaviour[] _scriptsToDisable;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] Animator _animator;

    private bool _isPaused;

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        Debug.Log(PauseManager.Instance);
        PauseManager.Instance.OnPaused += OnPause;
        PauseManager.Instance.OnResumed += OnResume;
    }

    private void OnDisable()
    {
        PauseManager.Instance.OnPaused -= OnPause;
        PauseManager.Instance.OnResumed -= OnResume;
    }

    public void OnPause()
    {
        Debug.Log("Player Paused");

        _playerMovement._controls.Player.Disable();
        foreach (MonoBehaviour script in _scriptsToDisable)
        {
            script.enabled = false;
        }

        _isPaused = true;
        _animator.speed = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnResume()
    {
        Debug.Log("Player Resumed");

        _playerMovement._controls.Player.Enable();
        foreach (MonoBehaviour script in _scriptsToDisable)
        {
            script.enabled = true;
        }


        _isPaused = false;
        _animator.speed = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
