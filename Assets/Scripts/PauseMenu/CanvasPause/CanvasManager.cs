using Patterns.Observer.EventManager_Delegates;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
   // [SerializeField] private GameObject canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // EventManager.SubscribeToEvent(EventsType.Event_PauseGame, EnableCanvas);
       // EventManager.SubscribeToEvent(EventsType.Event_ResumeGame, DisableCanvas);
    }
    
   // public void EnableCanvas(params object[] param)
   // {
      // canvas.SetActive(true);
   // }

   // public void DisableCanvas(params object[] param)
   // {
       // canvas.SetActive(false);
  //  }

}
