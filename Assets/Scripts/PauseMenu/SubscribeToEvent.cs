using Patterns.Observer.EventManager_Delegates;
using UnityEngine;
using System.Collections.Generic;

public class SubscribeToEvent : MonoBehaviour
{
    private IPause<Rigidbody> _pauseRb;
    private IPause<MonoBehaviour> _pauseNonRb;
    private IUnpause<Rigidbody> _unPauseRb;
    private IUnpause<MonoBehaviour> _unPauseNonRb;

    private List<Rigidbody> _rigidbodies;
    private List<MonoBehaviour> _monoBehaviours;

    public static SubscribeToEvent Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 🔥 instancias de lógica
        _pauseRb = new PauseRb();
        _pauseNonRb = new PauseNonRb();
        _unPauseRb = new UnPauseRb();
        _unPauseNonRb = new UnPauseNonRb();

        // 🔥 capturar escena
        _rigidbodies = new List<Rigidbody>(FindObjectsOfType<Rigidbody>());
        _monoBehaviours = new List<MonoBehaviour>(FindObjectsOfType<MonoBehaviour>());
    }

    private void Start()
    {
        

        EventManager.SubscribeToEvent(
            EventsType.Event_PauseGame,
            (parameters) =>
            {
                _pauseRb.PauseHandler(_rigidbodies);
            }
        );

        EventManager.SubscribeToEvent(
            EventsType.Event_PauseGame,
            (parameters) =>
            {
                _pauseNonRb.PauseHandler(_monoBehaviours);
            }
        );

        // =========================
        // 🟢 RESUME
        // =========================

        EventManager.SubscribeToEvent(
            EventsType.Event_ResumeGame,
            (parameters) =>
            {
                _unPauseRb.UnPauseHandler(_rigidbodies);
            }
        );

        EventManager.SubscribeToEvent(
            EventsType.Event_ResumeGame,
            (parameters) =>
            {
                _unPauseNonRb.UnPauseHandler(_monoBehaviours);
            }
        );
    }
}