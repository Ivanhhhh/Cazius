using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScreen
{
    /// <summary>
    /// Cuando agrego una pantalla, ejecuto este metodo para activarla
    /// </summary>
    void Activate(params object[] X);


    /// <summary>
    /// Cuando una nueva pantalla es agregada, la anterior debe ejecutar este metodo
    /// </summary>
    void Deactivate(params object[] X);

    /// <summary>
    /// Cuando una pantalla va a ser "destruida", ejecutamos este metodo
    /// </summary>
    void Release(params object[] X);
}
