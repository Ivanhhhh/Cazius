using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class Tornado : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El objeto al que seguirá durante la anticipación (ej. el jugador).")]
    [SerializeField] private Transform targetToFollow;

    [Header("Visual Effects")]
    [Tooltip("GameObject del Visual Effect 1 (Anticipación)")]
    [SerializeField] private VisualEffect anticipationVFX;
    [Tooltip("GameObject del Visual Effect 2 (Ataque)")]
    [SerializeField] private VisualEffect attackVFX;

    [Header("Colliders")]
    [Tooltip("Arrastra aquí los 4 GameObjects que contienen los colliders.")]
    [SerializeField] private GameObject[] colliders;

    [Header("Configuración de Tiempos y Movimiento")]
    [SerializeField] private float anticipationTime = 3f; // Tiempo que dura la anticipación
    [SerializeField] private float followSpeed = 10f;     // Velocidad a la que sigue al objetivo
    [Tooltip("Altura extra en Y para el seguimiento.")]
    [SerializeField] private float followYOffset = 1.5f;

    [Header("Configuración del Ataque")]
    [Tooltip("Tiempo manual que durará el Visual Effect del ataque antes de apagarse.")]
    [SerializeField] private float attackVFXDuration = 2f;

    private bool _isFollowing = false;
    private Vector3 _followOffset;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            targetToFollow = GameManager.Instance.Player.transform;
        }

        StartCoroutine(SequenceRoutine());
    }

    void LateUpdate()
    {
        if (_isFollowing && targetToFollow != null)
        {
            // Vamos directo a la posición del jugador, sin sumar distancias iniciales
            Vector3 targetPosition = targetToFollow.position;

            // Le sumamos el offset de altura
            targetPosition.y += followYOffset;

            // Lerp para que el movimiento sea fluido
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }

    private IEnumerator SequenceRoutine()
    {
        // 1. Empezamos a seguir al objetivo
        _isFollowing = true;

        // 2. Encendemos el primer Visual Effect
        if (anticipationVFX != null)
        {
            anticipationVFX.gameObject.SetActive(true);
        }

        // 3. Esperamos el tiempo de anticipación
        yield return new WaitForSeconds(anticipationTime);

        // 4. Dejamos de seguir al objetivo
        _isFollowing = false;

        // 5. Apagamos el primer VFX esperando a que mueran sus partículas
        if (anticipationVFX != null)
        {
            yield return StartCoroutine(WaitAndDisableVFX(anticipationVFX));
        }
        Debug.Log("activa ataque");
        // 6. Encendemos el segundo Visual Effect y los 4 colliders
        if (attackVFX != null)
        {
            attackVFX.gameObject.SetActive(true);
        }

        foreach (GameObject col in colliders)
        {
            if (col != null) col.SetActive(true);
        }

        // 7. Esperamos el "cooldown" manual del ataque
        yield return new WaitForSeconds(attackVFXDuration);

        // 8. Apagamos el segundo VFX de golpe, los colliders y el objeto principal
        if (attackVFX != null)
        {
            attackVFX.gameObject.SetActive(false);
        }

        foreach (GameObject col in colliders)
        {
            if (col != null) col.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    // Este método se queda solo para el VFX de anticipación (espera a que las partículas mueran)
    private IEnumerator WaitAndDisableVFX(VisualEffect vfx)
    {
        if (vfx == null) yield break;

        // Detiene la emisión si es un efecto en bucle


        vfx.gameObject.SetActive(false);
    }
}