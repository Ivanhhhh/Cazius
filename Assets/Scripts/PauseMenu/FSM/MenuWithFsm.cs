using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class MenuWithFSM : MonoBehaviour
{
    FSM<AgentStates> _fsm;


    void Awake()
    {
        _fsm = new FSM<AgentStates>();

        _fsm.AddState(AgentStates.Pause, new PauseState(_fsm));
        _fsm.AddState(AgentStates.Unpause, new UnPauseState(_fsm));

        _fsm.ChangeState(AgentStates.Unpause);
    }


    void Update() => _fsm.ArtificialUpdate();
}
