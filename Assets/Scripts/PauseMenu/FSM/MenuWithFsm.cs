using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class MenuWithFSM : MonoBehaviour
{
    FSM<AgentStates> _fsm;

    void Awake()
    {
        _fsm = new FSM<AgentStates>();

        _fsm.AddState(AgentStates.Pause, new PauseState(_fsm));
        _fsm.AddState(AgentStates.Unpause, new UnPauseState(_fsm));
        _fsm.AddState(AgentStates.Quit, new QuitState(_fsm));

        _fsm.ChangeState(AgentStates.Pause);
    }


    void Update() => _fsm.ArtificialUpdate();


}
