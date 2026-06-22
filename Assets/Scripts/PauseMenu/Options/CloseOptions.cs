using UnityEngine;
using UnityEngine.InputSystem;

public class CloseOptions : MonoBehaviour
{
    [SerializeField] PauseInputHandler _PauseInputHandler;

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                _PauseInputHandler.OnPause(default);

                this.gameObject.SetActive(false);
            }
        }
    }
}
