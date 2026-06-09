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
    [SerializeField] private float _dragSensitivity ; 
    [SerializeField] private float _smoothSpeed ; 

    [Header("Map Boundaries")]
    [SerializeField] private bool _useBoundaries = true;
    [SerializeField] private float _minX = -50f;
    [SerializeField] private float _maxX = 50f;
    [SerializeField] private float _minZ = -50f;
    [SerializeField] private float _maxZ = 50f;

    [Header("Zoom Settings")]
    [SerializeField] private float _minZoom ;   // Lo más cerca que puedes ver
    [SerializeField] private float _maxZoom;  // Lo más lejos que puedes ver
    [SerializeField] private float _zoomSensitivity ; // Qué tan rápido hace zoom la rueda
    [SerializeField] private float _zoomSmoothSpeed ;  // Suavizado del efecto "colchón"

    private bool _isDragging = false; 
    private Vector3 _targetPosition; 
    private float _targetZoom; // La variable destino para el Lerp del Zoom

    void OnEnable()
    {
        if (_player != null && _mapCamera != null)
        {
            SnapToPlayer();
            
            // Al abrir el mapa, igualamos el zoom destino con el actual
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
        
        // Nos aseguramos de que el jugador no esté fuera de los límites al hacer Snap
        ClampTargetPosition();
        
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

        // Aplicamos la restricción inmediatamente después de mover
        ClampTargetPosition();
    }

    public void StopDragging()
    {
        _isDragging = false;
    }

    private void ClampTargetPosition()
    {
        if (!_useBoundaries) return;

        _targetPosition.x = Mathf.Clamp(_targetPosition.x, _minX, _maxX);
        _targetPosition.z = Mathf.Clamp(_targetPosition.z, _minZ, _maxZ);
    }

    public void ZoomMap(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        if (pointerData == null) return;

        float scroll = pointerData.scrollDelta.y;

        _targetZoom -= scroll * _zoomSensitivity;
        _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
    }

    private void OnDrawGizmos()
    {
        if (!_useBoundaries) return;

        Gizmos.color = Color.cyan;

        float centerX = (_minX + _maxX) / 2f;
        float centerZ = (_minZ + _maxZ) / 2f;
        
        // Mantener el Gizmo a la altura de la cámara para que sea visible desde arriba
        float heightY = _mapCamera != null ? _mapCamera.transform.position.y : transform.position.y;
        
        Vector3 center = new Vector3(centerX, heightY, centerZ);
        Vector3 size = new Vector3(_maxX - _minX, 0.1f, _maxZ - _minZ);

        Gizmos.DrawWireCube(center, size);
    }
}