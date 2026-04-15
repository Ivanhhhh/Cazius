using Patterns.Observer.EventManager_Delegates;
using UnityEngine;
using System.Collections.Generic;

public class SubscribeToEvent : MonoBehaviour
{
    private PauseRb _pauseRb;
    private PauseNonRb _pauseNonRb;
    private UnPauseRb _unPauseRb;
    private UnPauseNonRb _unPauseNonRb;

    private List<Rigidbody> _rigidbodies;
    private List<MonoBehaviour> _monoBehaviours;

    public static SubscribeToEvent Instance { get; private set; }

    private void Awake()
    {
       

        // 🔥 instancias de lógicas
        _pauseRb = new PauseRb();   // cambiarlo a dip
        _pauseNonRb = new PauseNonRb();  // cambiarlo a dip
        _unPauseRb = new UnPauseRb();  // cambiarlo a dip
        _unPauseNonRb = new UnPauseNonRb();  // cambiarlo a dip


        // se llenan las listas con los rigidbpdys y scripts de la escena
        _rigidbodies = new List<Rigidbody>(FindObjectsOfType<Rigidbody>());
        _monoBehaviours = new List<MonoBehaviour>(FindObjectsOfType<MonoBehaviour>());
    }

    private void Start()
    {
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, (parameters) => _pauseRb.PauseHandler(_rigidbodies));
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, (parameters) => _pauseNonRb.PauseHandler(_monoBehaviours));

        // =========================
        // 🟢 RESUME
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, (parameters) => _unPauseRb.UnPauseHandler(_rigidbodies));
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, (parameters) => _unPauseNonRb.UnPauseHandler(_monoBehaviours));
    }
}