using System.Collections;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }
    public PlayerControls Controls { get; private set; }

    [SerializeField] private float _afterDialogReleaseSecs = 0.5f;

    private Coroutine _enablePlayerCoroutine;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Controls = new PlayerControls();
        Controls.Player.Enable();
        Controls.UI.Enable();
        Controls.Dialog.Disable();
    }

    public void DisablePlayerMovement()
    {
        Controls.Player.Disable();
    }

    public void EnablePlayerMovement()
    {
        Controls.Player.Enable();
    }

    public void EnableDialogInput()
    {
        StopPendingEnablePlayer();
        Controls.Player.Disable();
        Controls.UI.Disable();
        Controls.Dialog.Enable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableDialogInput()
    {
        Controls.Dialog.Disable();
        Controls.UI.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StopPendingEnablePlayer();
        _enablePlayerCoroutine = StartCoroutine(EnablePlayerNextFrame());
    }

    public void EnableShopInput()
    {
        StopPendingEnablePlayer();
        Controls.Player.Disable();
        Controls.UI.Disable();
        Controls.Dialog.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableShopInput()
    {
        Controls.UI.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StopPendingEnablePlayer();
        _enablePlayerCoroutine = StartCoroutine(EnablePlayerNextFrame());
    }

    private void StopPendingEnablePlayer()
    {
        if (_enablePlayerCoroutine != null)
        {
            StopCoroutine(_enablePlayerCoroutine);
            _enablePlayerCoroutine = null;
        }
    }

    private IEnumerator EnablePlayerNextFrame()
    {
        yield return new WaitForSeconds(_afterDialogReleaseSecs);
        Controls.Player.Enable();
        _enablePlayerCoroutine = null;
    }

    void OnDestroy() => Controls.Dispose();

}
