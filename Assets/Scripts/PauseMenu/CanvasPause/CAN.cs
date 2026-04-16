using Patterns.Observer.EventManager_Delegates;
using UnityEngine;
using UnityEngine.InputSystem;

public class CAN : MonoBehaviour
{
    [SerializeField] InputActionReference reference;

    FSM<AgentStates> _fsm;

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
            EventManager.TriggerEvent(EventsType.Event_PauseGame);

           //_fsm.ChangeState(AgentStates.Pause);

            print("jfff");
        }
    }
}