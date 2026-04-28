using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class InventoryScreen : MonoBehaviour,IScreen
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
    }
   public void Activate(params object[] x)
   {
     this.gameObject.SetActive(true);
        EventManager.UnsubscribeToEvent(EventsType.Event_PauseGame, Activate);
        SubscribeToEvent();
    }


    /// <summary>
    /// Cuando una nueva pantalla es agregada, la anterior debe ejecutar este metodo
    /// </summary>
   public void Deactivate(params object[] x)
   {
     this.gameObject.SetActive(false);
        EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, Deactivate);
    }

    /// <summary>
    /// Cuando una pantalla va a ser "destruida", ejecutamos este metodo
    /// </summary>
   public void Release(params object[] x)
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
