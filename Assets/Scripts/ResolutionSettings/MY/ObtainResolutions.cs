using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObtainResolutions : MonoBehaviour
{
   public List <Resolution>  SetupResolutions() // creo el metodo SetupResolutions
    {
        List<Resolution> _availableResolutions = new List<Resolution>(); // creo una lista de resoluciones

        Resolution[] resolutions = Screen.resolutions;  // un array de resoluciones

        HashSet<string> addedResolutions = new HashSet<string>(); //creo un hashset para de resoluciones agregadas para que no se repitan resoluciones ya que un set no permite duplicados

        foreach (var item in resolutions)  // por cada resolucion dentrro del array de resolutions
        {
          string key = item.width + "x" + item.height; //un simple string

          if (addedResolutions.Contains(key))
            {
                continue;
            }

             _availableResolutions.Add(item); // agrego a la coleccion de avaible resolutions una resolucion nueva
             addedResolutions.Add(key);

             
        }
         return _availableResolutions;
    }
}
