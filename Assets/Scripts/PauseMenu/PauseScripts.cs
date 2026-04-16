using NUnit.Framework;
using Patterns.Observer.EventManager_Delegates;
using System.Collections.Generic;
using UnityEngine;

public class PauseScripts : MonoBehaviour,IPausable
{
    private List<MonoBehaviour> ScriptsList;
    void Start()
    {
        ScriptsList = new List<MonoBehaviour>();
        ScriptsList.AddRange(GetComponentsInParent<MonoBehaviour>());

        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, Pause);
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, UnPause);
    }

    public void Pause(params object[] param)
    {
        foreach (MonoBehaviour Script in ScriptsList)
        {
          Script.enabled = false;
        }
    }

    public void UnPause(params object[] param)
    {
        foreach (MonoBehaviour Script in ScriptsList)
        {
            Script.enabled = true;
        }
    }   
}
