using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PauseRb :IPause<Rigidbody>
{
    private List<Rigidbody> _AllElements = new List<Rigidbody>();

    public void PauseHandler(List<Rigidbody> ElementsToPause)
    {

        foreach (var element in ElementsToPause)
        {
          element.isKinematic = true;
                // is kinematic == true
        }
    }
}
