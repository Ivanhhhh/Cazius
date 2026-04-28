using UnityEngine;
using UnityEngine.InputSystem;

public class InputToggle : MonoBehaviour
{

    [SerializeField] GameObject _inventoryObject;
    [SerializeField] GameObject _pauseObject;

    IScreen _inventoryScreen;
    IScreen _pauseScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // screen = GetComponent<IScreen>();

        
    }

    private void OnEnable()
    {
        _inventoryScreen = _inventoryObject.GetComponent<IScreen>();
        _pauseScreen = _pauseObject.GetComponent<IScreen>();
    }



    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            PauseManager.Instance.Toggle();
            ScreenManager.Instance.AddNewScreen(_inventoryScreen);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseManager.Instance.Toggle();
            if (PauseManager.Instance._isPaused)
                ScreenManager.Instance.AddNewScreen(_pauseScreen);
            else
                ScreenManager.Instance.RemoveLastScreen();
        }
    }

}
