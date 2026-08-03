using UnityEngine;
using UnityEngine.Animations.Rigging;
public class Player_IK_modifier : MonoBehaviour
{
    [Header("Referencias Principales")]
    public Animator animator;
    public Rig brazoIK;

    [Header("Configuración del Bool")]
    [Tooltip("Nombre exacto del parámetro Bool en el Animator.")]
    public string nombreBoolAnimator;

    [Header("Configuración de Suavizado")]
    [Tooltip("Velocidad constante del cambio de weight (unidades por segundo).")]
    public float velocidadTransicion = 2f;

    private float pesoObjetivo = 1f;

    void Update()
    {
        if (animator == null || brazoIK == null) return;

        bool boolActivo = animator.GetBool(nombreBoolAnimator);

        pesoObjetivo = boolActivo ? 0f : 1f;

        brazoIK.weight = Mathf.MoveTowards(brazoIK.weight, pesoObjetivo, Time.deltaTime * velocidadTransicion);
    }
}