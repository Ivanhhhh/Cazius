using Patterns.Observer.EventManager_Delegates;
using UnityEngine;
using UnityEngine.InputSystem;

public class CAN : MonoBehaviour
{
    [SerializeField] InputActionReference reference;
    [SerializeField] Canvas canvas;

    DisableCanvas disableCanvas = new DisableCanvas();

    void OnEnable()
    {
        reference.action.Enable();
    }

    void OnDisable()
    {
        reference.action.Disable();
    }

    void Update()
    {
        if (reference.action.triggered)
        {
            EventManager.TriggerEvent(EventsType.Event_DisableCanvas,canvas);
            print("jfff");
        }
    }
}