using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

   public bool _isPaused;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Toggle()
    {
        if (_isPaused == true) Unpause();
        else Pause();
        print (_isPaused);
    }

   public void Pause()
   {
        _isPaused = true;
        EventManager.TriggerEvent(EventsType.Event_PauseGame);
   }

   public void Unpause()
   {
        _isPaused = false;
        EventManager.TriggerEvent(EventsType.Event_ResumeGame);
   }
}