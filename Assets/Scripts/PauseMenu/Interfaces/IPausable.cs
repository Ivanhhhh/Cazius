using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public interface IPausable
{
    public void Pause(params object[] _);

    public void UnPause(params object[] _);
}
