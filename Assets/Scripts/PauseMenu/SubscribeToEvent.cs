using Patterns.Observer.EventManager_Delegates;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SubscribeToEvent : MonoBehaviour
{
    private PauseRb _pauseRb;   // cambiarlo a dip
    private PauseNonRb _pauseNonRb;  // cambiarlo a dip
    private UnPauseRb _unPauseRb;   // cambiarlo a dip
    private UnPauseNonRb _unPauseNonRb;  // cambiarlo a dip
    private UnableCanvas _UnableCanvas;  // cambiarlo a dip
    private DisableCanvas _DisableCanvas;  // cambiarlo a dip


    private List<Rigidbody> _rigidbodies;
    private List<MonoBehaviour> _monoBehaviours;

  //  [SerializeField] private string CanvasName;
    Canvas PauseMenu;

   // [SerializeField] InputActionReference Input;

    public static SubscribeToEvent Instance { get; private set; }

    private void Awake()
    {
        //PauseMenu = GameObject.Find(CanvasName).GetComponent<Canvas>();
        _pauseRb = new PauseRb();   // cambiarlo a dip
        _pauseNonRb = new PauseNonRb();  // cambiarlo a dip
        _unPauseRb = new UnPauseRb();  // cambiarlo a dip
        _unPauseNonRb = new UnPauseNonRb();  // cambiarlo a dip
        _UnableCanvas = new UnableCanvas();   // cambiarlo a dip
        _DisableCanvas = new DisableCanvas();   // cambiarlo a dip


        _rigidbodies = new List<Rigidbody>();
        _monoBehaviours = new List<MonoBehaviour>();

        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, (parameters) => _pauseRb.PauseHandler(_rigidbodies));
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, (parameters) => _pauseNonRb.PauseHandler(_monoBehaviours));
        EventManager.SubscribeToEvent(EventsType.Event_UnableCanvas, (parameters) => _UnableCanvas.UnableCanvasMethod(PauseMenu));       


       var FindRb = GetComponent<Rigidbody>();
        _rigidbodies.Add(FindRb);
        var FindMono = GetComponentsInParent<MonoBehaviour>();
        
        foreach (var x in FindMono)
        {
            _monoBehaviours.Add(x);
        }

    }

    private void Start()
    {
        
        // =========================
        // 🟢 RESUME
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, (parameters) => _unPauseRb.UnPauseHandler(_rigidbodies));
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, (parameters) => _unPauseNonRb.UnPauseHandler(_monoBehaviours));
        EventManager.SubscribeToEvent(EventsType.Event_DisableCanvas, (parameters) => _DisableCanvas.DisableCanvasMethod(PauseMenu));


    }
}