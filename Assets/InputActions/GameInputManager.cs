using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }
    public PlayerControls Controls { get; private set; }

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

    public void EnableDialogInput()
    {
        Controls.Player.Disable();
        Controls.UI.Disable();
        Controls.Dialog.Enable();
    }

    public void DisableDialogInput()
    {
        Controls.Dialog.Disable();
        Controls.UI.Enable();
        Controls.Player.Enable();
    }

    void OnDestroy() => Controls.Dispose();
}