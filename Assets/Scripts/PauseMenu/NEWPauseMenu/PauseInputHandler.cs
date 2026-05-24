using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputHandler : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuCanvas;

    void Start() => GameInputManager.Instance.Controls.UI.PauseMenu.performed += OnPause;

    void OnDisable() => GameInputManager.Instance.Controls.UI.PauseMenu.performed -= OnPause;

    public void OnPause(InputAction.CallbackContext _)
    {
        if (_pauseMenuCanvas.activeSelf)
        {
            PauseManager.Instance.Toggle();
            _pauseMenuCanvas.SetActive(false);
            return;
        }

        if (PauseManager.Instance.IsPaused) return;

        PauseManager.Instance.Toggle();
        _pauseMenuCanvas.SetActive(true);
    }

}
