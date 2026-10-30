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

    [Header("Ataque con Shader _Dissolve")]
    [Tooltip("El GameObject que tiene el Renderer con el Shader.")]
    [SerializeField] private GameObject attackVisualObject;
    [SerializeField] private string dissolvePropertyName = "_Dissolve";
    [SerializeField] private float dissolveStartValue = 7f;
    [SerializeField] private float dissolveEndValue = 60f;

    [Tooltip("Qué tan rápido el dissolve sube de 7 a 60.")]
    [SerializeField] private float dissolveSpeedIn = 15f;

    [Tooltip("Qué tan rápido el dissolve baja de 60 a 7 después de que los objetos desaparezcan.")]
    [SerializeField] private float dissolveSpeedOut = 10f;

    [Header("Objetos a Activar en el Pico del Ataque")]
    [Tooltip("Los dos objetos que se activarán cuando el dissolve llegue a 60 y esperaremos a que desaparezcan.")]
    [SerializeField] private GameObject[] objectsToActivateAfterDissolveIn;

    [Header("Colliders del Ataque")]
    [Tooltip("Arrastra aquí los GameObjects que contienen los colliders del ataque.")]
    [SerializeField] private GameObject[] colliders;

    [Header("Configuración de Tiempos y Movimiento")]
    [SerializeField] private float anticipationTime = 3f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float followYOffset = 1.5f;

    private bool _isFollowing = false;
    private Renderer _attackRenderer;
    private MaterialPropertyBlock _propertyBlock;
    private float _currentDissolve;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            targetToFollow = GameManager.Instance.Player.transform;
        }

        // Preparar el Renderer para modificar el shader sin romper el material original
        if (attackVisualObject != null)
        {
            _attackRenderer = attackVisualObject.GetComponentInChildren<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }
        else
        {
            Debug.LogWarning("Falta asignar el attackVisualObject en el Inspector.");
        }

        StartCoroutine(SequenceRoutine());
    }

    void LateUpdate()
    {
        if (_isFollowing && targetToFollow != null)
        {
            Vector3 targetPosition = targetToFollow.position;
            targetPosition.y += followYOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }

    private IEnumerator SequenceRoutine()
    {
        // --- 1. FASE DE ANTICIPACIÓN ---
        _isFollowing = true;

        if (anticipationVFX != null)
        {
            anticipationVFX.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(anticipationTime);

        // --- 2. INICIO DEL ATAQUE ---
        _isFollowing = false;

        if (anticipationVFX != null)
        {
            anticipationVFX.gameObject.SetActive(false); // O usar WaitAndDisableVFX si prefieres esperar a las partículas
        }

        Debug.Log("Iniciando ataque con Dissolve");

        // Encender el objeto visual del ataque
        if (attackVisualObject != null)
        {
            attackVisualObject.SetActive(true);
        }

        // Encender los colliders INMEDIATAMENTE
        SetCollidersState(true);

        // --- 3. SUBIR EL DISSOLVE (7 a 60) ---
        _currentDissolve = dissolveStartValue;
        SetDissolveValue(_currentDissolve);

        while (_currentDissolve < dissolveEndValue)
        {
            _currentDissolve = Mathf.MoveTowards(_currentDissolve, dissolveEndValue, dissolveSpeedIn * Time.deltaTime);
            SetDissolveValue(_currentDissolve);
            yield return null; // Esperar al siguiente frame
        }

        // --- 4. ACTIVAR LOS DOS OBJETOS ---
        Debug.Log("Dissolve llegó a 60. Activando objetos secundarios.");
        if (objectsToActivateAfterDissolveIn != null)
        {
            foreach (GameObject obj in objectsToActivateAfterDissolveIn)
            {
                if (obj != null) obj.SetActive(true);
            }
        }

        // --- 5. ESPERAR A QUE LOS OBJETOS SEAN DESTRUIDOS O DESACTIVADOS ---
        // La corrutina se pausará acá hasta que TODOS los objetos de la lista estén nulos o inactivos.
        yield return new WaitUntil(() => AllObjectsAreGone());

        // --- 6. BAJAR EL DISSOLVE (60 a 7) ---
        Debug.Log("Objetos destruidos. Iniciando Dissolve Out.");

        while (_currentDissolve > dissolveStartValue)
        {
            _currentDissolve = Mathf.MoveTowards(_currentDissolve, dissolveStartValue, dissolveSpeedOut * Time.deltaTime);
            SetDissolveValue(_currentDissolve);
            yield return null;
        }

        // --- 7. LIMPIEZA FINAL ---
        SetCollidersState(false);

        if (attackVisualObject != null)
        {
            attackVisualObject.SetActive(false);
        }

        Debug.Log("Ataque terminado. Desactivando Tornado.");
        gameObject.SetActive(false);
    }

    // --- MÉTODOS AUXILIARES ---

    private void SetDissolveValue(float value)
    {
        if (_attackRenderer != null && _propertyBlock != null)
        {
            _attackRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(dissolvePropertyName, value);
            _attackRenderer.SetPropertyBlock(_propertyBlock);
        }
    }

    private void SetCollidersState(bool state)
    {
        if (colliders == null) return;

        foreach (GameObject col in colliders)
        {
            if (col != null) col.SetActive(state);
        }
    }

    // Devuelve true solo si TODOS los objetos están destruidos o desactivados
    private bool AllObjectsAreGone()
    {
        if (objectsToActivateAfterDissolveIn == null || objectsToActivateAfterDissolveIn.Length == 0)
            return true;

        foreach (GameObject obj in objectsToActivateAfterDissolveIn)
        {
            // Si el objeto existe en memoria y está activo, entonces no están todos "gone"
            if (obj != null && obj.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }
}