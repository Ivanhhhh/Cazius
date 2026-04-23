using NUnit.Framework;
using Patterns.Observer.EventManager_Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PauseScripts : MonoBehaviour,IPausable
{
    private List<MonoBehaviour> ScriptsList;
    //[SerializeField] List<MonoBehaviour> ScriptsException;
    private void Awake()
    {
        ScriptsList = new List<MonoBehaviour>();

        ScriptsList.AddRange(GetComponentsInParent<MonoBehaviour>());


        var FilterList = ScriptsList.Where(x => x.enabled == true).ToList();

        ScriptsList = FilterList;
    }

    void OnEnable ()
    {
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, Pause);
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame,UnPause);   
    }

    void OnDisable ()
    {
        EventManager.UnsubscribeToEvent(EventsType.Event_PauseGame, Pause);
        EventManager.UnsubscribeToEvent(EventsType.Event_ResumeGame, UnPause);
    }

    public void Pause(params object[] param)
    {
        foreach (MonoBehaviour Script in ScriptsList)
        {
           if (Script == this) continue;
           // if (ScriptsException.Contains continue;


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
