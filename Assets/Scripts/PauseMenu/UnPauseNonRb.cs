using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnPauseNonRb : IUnpause<MonoBehaviour>
{
    private List<MonoBehaviour> _AllElements = new List<MonoBehaviour>();

    public void UnPauseHandler(List<MonoBehaviour> ElementsToPause)
    {
     
        foreach (var element in ElementsToPause)
        {
            element.enabled = true;
          //element.isKinematic = true;
                // is kinematic == true
        }
    }
}
