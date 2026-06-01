using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOption : MonoBehaviour
{
   private (Button last, Button current) tupleButtons; // una tupla para que al apretar un boton se desactiva el canvas del boton anterior y se activa el del que aprete recien (se activa el de current y el de last se desactiva)
   private  CloseOptions closeOptions; // referencia a la clase que a alpretar un boton el mismo s epone en current de la tupla.

    private Canvas _canvas1; // variable donde se almacenan el boton current y el last (se mantiene la referencia).
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

            _canvas2 = tupleButtons.last.GetComponentInChildren<Canvas>();
            _canvas2.enabled = false;
            

            _canvas1 = tupleButtons.current.GetComponentInChildren<Canvas>();
            _canvas1.enabled = true;

        }
    }
}