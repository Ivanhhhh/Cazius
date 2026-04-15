using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnPauseRb :IUnpause<Rigidbody>
{
    private List<Rigidbody> _AllElements = new List<Rigidbody>();

    public void UnPauseHandler(List<Rigidbody> ElementsToPause)
    {

        foreach (var element in ElementsToPause)
        {
            if (element != null)
            {
                element.isKinematic = false;
            }
            // is kinematic == true
        }
    }
}
