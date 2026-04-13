using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public interface IPause<T>
{
    public void PauseHandler(List<T> ElementsToPause);
}
