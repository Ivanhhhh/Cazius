using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // ¡OBLIGATORIO para detectar datos del mouse!
public class MapCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _player;
    
    [Header("Camera Parameters")]
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private RawImage _mapDisplay;
    
    [Header("Drag Settings")]
    [SerializeField] private float _dragSensitivity = 0.05f; 
    [SerializeField] private float _smoothSpeed = 10f; 

    [Header("Zoom Settings")]
    [SerializeField] private float _minZoom = 5f;   // Lo más cerca que puedes ver
    [SerializeField] private float _maxZoom = 25f;  // Lo más lejos que puedes ver
    [SerializeField] private float _zoomSensitivity = 1.5f; // Qué tan rápido hace zoom la rueda
    [SerializeField] private float _zoomSmoothSpeed = 10f;  // Suavizado del efecto "colchón"

    private bool _isDragging = false; 
    private Vector3 _targetPosition; 
    private float _targetZoom; // La variable destino para el Lerp del Zoom

    void OnEnable()
    {
        if (_player != null && _mapCamera != null)
        {
            SnapToPlayer();
            
            // Al abrir el mapa, igualamos el zoom destino con el actual
            // NOTA: Si tu cámara es 3D y usa Field Of View, cambia "orthographicSize" por "fieldOfView"
            _targetZoom = _mapCamera.orthographicSize; 
        }
    }

    void LateUpdate()
    {
    if (_player == null || _mapCamera == null) return;


    // 1. Calculamos el factor de suavizado matemáticamente perfecto para cualquier FPS
    float moveBlend = 1f - Mathf.Exp(-_smoothSpeed * Time.unscaledDeltaTime);
    float zoomBlend = 1f - Mathf.Exp(-_zoomSmoothSpeed * Time.unscaledDeltaTime);

    // 2. Aplicamos el factor a nuestros Lerps
    _mapCamera.transform.position = Vector3.Lerp(_mapCamera.transform.position, _targetPosition, moveBlend);
    _mapCamera.orthographicSize = Mathf.Lerp(_mapCamera.orthographicSize, _targetZoom, zoomBlend);
}
    public void SnapToPlayer()
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 cameraPos = _mapCamera.transform.position;
        
        _targetPosition = new Vector3(playerPos.x, cameraPos.y, playerPos.z);
        _mapCamera.transform.position = _targetPosition;
    }

    public void StartDragging()
    {
        _isDragging = true;
    }

    public void DragMap(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        if (pointerData == null) return;

        float moveX = -pointerData.delta.x * _dragSensitivity;
        float moveZ = -pointerData.delta.y * _dragSensitivity;

        _targetPosition += new Vector3(moveX, 0, moveZ);
    }

    public void StopDragging()
    {
        _isDragging = false;
    }

    // ==========================================
    // 🔍 NUEVA FUNCIÓN PARA EL EVENT TRIGGER
    // ==========================================
    public void ZoomMap(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        if (pointerData == null) return;

        // scrollDelta.y nos da positivo si giramos hacia adelante, negativo si es hacia atrás
        float scroll = pointerData.scrollDelta.y;

        // Restamos el valor: Girar hacia adelante (positivo) achica la cámara (acerca la imagen)
        _targetZoom -= scroll * _zoomSensitivity;

        // Clamp asegura de que no nos pasemos de los límites máximo y mínimo
        _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
    }
}