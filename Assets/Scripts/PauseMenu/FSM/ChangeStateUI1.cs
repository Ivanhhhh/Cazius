using UnityEngine;
using Patterns.Observer.EventManager_Delegates;

public class ChangeStateUI1 : MonoBehaviour
{
   
    public void PauseGame()
    {
      EventManager.TriggerEvent(EventsType.Event_PauseGame);
    }


    public void UnPauseGame()
    {
        EventManager.TriggerEvent(EventsType.Event_ResumeGame);
    }



    public void CanvasDisable()
    {
        EventManager.TriggerEvent(EventsType.Event_UnableCanvas);
    }
}
