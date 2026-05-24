using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOption : MonoBehaviour
{
   private (Button last, Button current) tupleButtons;
   private  CloseOptions closeOptions;

    private Canvas _canvas1;
    private Canvas _canvas2;

    private void Start()
    {
        if (this.gameObject.activeSelf)
        {
            tupleButtons.current = GetComponentInChildren<Button>();

            foreach (var canvas in GetComponentsInChildren<Canvas>())
            {
                if (canvas.GetComponentInParent<Button>() != null)
                {
                    canvas.enabled = false;
                }
            } 
        }
    }


    public void ButtonToggle(Button PressedButton)
    {

        if (this.gameObject.activeSelf)
        {
            tupleButtons.last = tupleButtons.current; // el ultimo boton es el ultimo que aprete
            tupleButtons.current = PressedButton; // el boton actual es el que apreto ahora

            //Debug.Log("Pressed: " + PressedButton.name);
            //Debug.Log("Canvas del pressed: " + PressedButton.GetComponentInChildren<Canvas>());

            _canvas2 = tupleButtons.last.GetComponentInChildren<Canvas>();
               _canvas2.enabled = false;
            

            _canvas1 = tupleButtons.current.GetComponentInChildren<Canvas>();
            _canvas1.enabled = true;

        }
    }
}