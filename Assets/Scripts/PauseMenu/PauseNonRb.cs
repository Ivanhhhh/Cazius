using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PauseNonRb : IPause<MonoBehaviour>
{
    private List<MonoBehaviour> _AllElements = new List<MonoBehaviour>();

    public void PauseHandler(List<MonoBehaviour> ElementsToPause)
    {
     
        foreach (var element in ElementsToPause)
        {
            if (element.GetComponentInParent<Canvas>() != null)
                continue;

            element.enabled = false;
            Debug.Log("Se Hizo");
         
        }
    }
}
