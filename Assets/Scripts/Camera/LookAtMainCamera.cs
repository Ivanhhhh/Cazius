using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
    [Tooltip("Marca esto si quieres que solo gire en el eje Y (como una barra de vida).")]
    public bool lockYAxisOnly = false;

    private Camera _mainCamera;

    void Start()
    {
        // Guardamos la referencia para no buscarla cada frame (es costoso)
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (_mainCamera == null) return;

        if (lockYAxisOnly)
        {
            // Opción 1: Solo gira en el eje Y (ideal para barreras o UI en el mundo)
            Vector3 targetPosition = new Vector3(
                _mainCamera.transform.position.x,
                transform.position.y,
                _mainCamera.transform.position.z
            );
            transform.LookAt(targetPosition);
        }
        else
        {
            // Opción 2: Mira directamente a la cámara en 3D (Billboard completo)
            transform.LookAt(_mainCamera.transform.position);
        }
    }
}
