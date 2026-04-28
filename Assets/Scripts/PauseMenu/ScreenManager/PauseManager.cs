using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

   public bool _IsPaused;

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
        if (_IsPaused == true) Unpause();
        else Pause();
        print (_IsPaused);
    }

   public void Pause()
   {
        _IsPaused = true;
        EventManager.TriggerEvent(EventsType.Event_PauseGame);
   }

   public void Unpause()
   {
        _IsPaused = false;
        EventManager.TriggerEvent(EventsType.Event_ResumeGame);
   }
}