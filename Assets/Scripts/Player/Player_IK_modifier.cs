using UnityEngine;
using UnityEngine.Animations.Rigging;
public class Player_IK_modifier : MonoBehaviour
{
    [Header("Referencias Principales")]
    public Animator animator;
    public TwoBoneIKConstraint brazoIK;

    [Header("Configuración de Estados")]
    [Tooltip("Escribe exactamente el nombre del ESTADO en el Animator.")]
    public string nombreEstadoObjetivo;
    public int capaAnimator = 0;

    [Header("Configuración de Suavizado")]
    [Tooltip("Velocidad con la que el Target Position Weight vuelve a 1 al terminar o anticipar.")]
    public float velocidadTransicion = 10f;

    [Tooltip("Si está activo, el Target Position Weight pasará a 0 en el frame 1 de golpe sin suavizado.")]
    public bool apagarAlInstante = true;

    [Header("Anticipación del Final")]
    [Range(0f, 1f)]
    [Tooltip("En qué porcentaje de la animación empezará a volver a 1. 0.8 significa que al llegar al 80%.")]
    public float momentoRetorno = 0.8f;

    private float pesoObjetivo = 1f;

    void Update()
    {
        if (animator == null || brazoIK == null) return;

        AnimatorStateInfo estadoActual = animator.GetCurrentAnimatorStateInfo(capaAnimator);
        AnimatorStateInfo estadoSiguiente = animator.GetNextAnimatorStateInfo(capaAnimator);
        
        bool enEstadoActual = estadoActual.IsName(nombreEstadoObjetivo);
        bool enEstadoSiguiente = estadoSiguiente.IsName(nombreEstadoObjetivo);

        // 1. EXTRAEMOS la estructura de datos del constraint
        var datosIK = brazoIK.data;

        // CASO 1: Está entrando a la animación
        if (enEstadoSiguiente)
        {
            pesoObjetivo = 0f;
            if (apagarAlInstante) datosIK.targetPositionWeight = 0f;
        }
        // CASO 2: Ya está reproduciendo la animación principal
        else if (enEstadoActual)
        {
            float progresoAnimacion = estadoActual.normalizedTime % 1f;

            if (progresoAnimacion >= momentoRetorno)
            {
                pesoObjetivo = 1f; // Empieza a anticipar el final
            }
            else
            {
                pesoObjetivo = 0.4f; // Sigue apagado
                if (apagarAlInstante) datosIK.targetPositionWeight = 0f;
            }
        }
        // CASO 3: Completamente fuera de la animación objetivo
        else
        {
            pesoObjetivo = 1f;
        }

        // 2. MODIFICAMOS el Lerp aplicando el cálculo EXCLUSIVAMENTE al targetPositionWeight
        datosIK.targetPositionWeight = Mathf.Lerp(datosIK.targetPositionWeight, pesoObjetivo, Time.deltaTime * velocidadTransicion);
        
        // 3. REASIGNAMOS los datos modificados de vuelta al componente
        brazoIK.data = datosIK;
    }
}