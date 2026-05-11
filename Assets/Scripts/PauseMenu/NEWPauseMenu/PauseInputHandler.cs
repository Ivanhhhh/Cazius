using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference _pauseAction;
    [SerializeField] private GameObject _pauseMenuCanvas;

    void OnEnable() => _pauseAction.action.performed += OnPause;
    void OnDisable() => _pauseAction.action.performed -= OnPause;

    private void OnPause(InputAction.CallbackContext _)
    {
        if (_pauseMenuCanvas.activeSelf)
        {
            PauseManager.Instance.Toggle();
            _pauseMenuCanvas.SetActive(false);
            return;
        }

        if (PauseManager.Instance.IsPaused)
            return;

        PauseManager.Instance.Toggle();
        _pauseMenuCanvas.SetActive(true);
    }

}
