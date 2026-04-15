using Patterns.Observer.EventManager_Delegates;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class DisableCanvas
{
    public void DisableCanvasMethod(Canvas canvas)
    {
        //reference.action.performed += OnPressed; ;
        
           canvas.enabled = false;
        
    }
}
