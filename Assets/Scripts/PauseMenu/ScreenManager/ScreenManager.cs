using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    //Una coleccion que guarde cada pantalla entrante
    Stack<IScreen> _screensInUse;

    private void Awake()
    {
        Instance = this;
        _screensInUse = new Stack<IScreen>();
    }

    //Un metodo para agregar una nueva pantalla
    public void AddNewScreen(IScreen newScreen)
    {
        if (_screensInUse.Contains(newScreen)) return;

        if (_screensInUse.Count != 0)
        {
            var oldScreen = _screensInUse.Peek();
            oldScreen.Deactivate();
        }

        _screensInUse.Push(newScreen);
        newScreen.Activate();
    }

    //Un metodo para sacar la ultima pantalla
    public void RemoveLastScreen()
    {
        if (_screensInUse.Count <= 1) return;

        var screenToRelease = _screensInUse.Pop();
        screenToRelease.Release();

        var lastScreen = _screensInUse.Peek();
        lastScreen.Activate();
    }
}
