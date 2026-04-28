using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class PauseScreen : MonoBehaviour,IScreen
{

    void OnEnable()
    {
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, Activate);
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, Deactivate);
    }
    void OnDisable()
    {
        // EventManager.UnsubscribeToEvent(EventsType.Event_PauseGame, Activate);
        // EventManager.UnsubscribeToEvent(EventsType.Event_ResumeGame, Deactivate);

        UnsubscribeToEvent();
    }


    public void Activate(params object[] x)
    {
     this.gameObject.SetActive(true);
        SubscribeToEvent();

    }


    /// <summary>
    /// Cuando una nueva pantalla es agregada, la anterior debe ejecutar este metodo
    /// </summary>
   public void Deactivate(params object[] X)
   {
     this.gameObject.SetActive(false);
        
   }

    /// <summary>
    /// Cuando una pantalla va a ser "destruida", ejecutamos este metodo
    /// </summary>
   public void Release(params object[] X)
   {
     this.gameObject.SetActive(false);
   }

    public void UnsubscribeToEvent()
    {
        EventManager.UnsubscribeToEvent(EventsType.Event_PauseGame, Activate);
        EventManager.UnsubscribeToEvent(EventsType.Event_ResumeGame, Deactivate);

    }


    public void SubscribeToEvent()
    {
        EventManager.SubscribeToEvent(EventsType.Event_PauseGame, Activate);
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, Deactivate);
    }
}
