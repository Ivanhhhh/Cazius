using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public interface IUnpause<T>
{
    public void UnPauseHandler(List<T> ElementsToUnPause);
}
