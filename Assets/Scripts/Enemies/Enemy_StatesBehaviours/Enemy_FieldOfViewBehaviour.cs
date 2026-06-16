using UnityEngine;

public class Enemy_FieldOfViewBehaviour
{
    private Transform _playerTransform;
    private float _radiusVision;
    
    private float _horizontalAngleVision;
    private float _verticalAngleVision;
    
    private Transform _objectTransform;
    private LayerMask _lineOfSightLayerMask;
    
    // Nueva variable para unificar la altura de visión
    private float _aimOffset; 

    public Enemy_FieldOfViewBehaviour(
        Transform playerTransform, 
        float radiusVision, 
        Transform objectTransform, 
        float horizontalAngleVision, 
        float verticalAngleVision, 
        LayerMask lineOfSightLayerMask,
        float aimOffset) // Pasamos el offset por el constructor
    {
        _playerTransform = playerTransform;
        _radiusVision = radiusVision;
        _objectTransform = objectTransform;
        _horizontalAngleVision = horizontalAngleVision;
        _verticalAngleVision = verticalAngleVision;
        _lineOfSightLayerMask = lineOfSightLayerMask;
        
        _aimOffset = aimOffset; 
    }

    public bool CanseePlayer()
    {
        if (_playerTransform == null) return false;
        return InFieldOfView(_playerTransform.position);
    }

    bool InFieldOfView(Vector3 endPos)
    {
        // 1. Usamos la variable unificada
        Vector3 targetCenter = endPos + (Vector3.up * _aimOffset);

        // 2. Calculamos la dirección
        Vector3 dir = targetCenter - _objectTransform.position;
        if (dir.magnitude > _radiusVision) return false;

        Vector3 localDir = _objectTransform.InverseTransformDirection(dir);

        float horizontalAngle = Mathf.Abs(Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg);
        float verticalAngle = Mathf.Abs(Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg);

        if (horizontalAngle > _horizontalAngleVision / 2f || verticalAngle > _verticalAngleVision / 2f) 
        {
            return false;
        }

        if (!InLineOfSight(_objectTransform.position, endPos)) return false;
        
        return true;
    }

    bool InLineOfSight(Vector3 startPos, Vector3 endPos)
    {
        // Usamos exactamente LA MISMA variable para el raycast
        Vector3 targetCenter = endPos + (Vector3.up * _aimOffset); 
        Vector3 dir = targetCenter - startPos;
        
        return !Physics.Raycast(startPos, dir.normalized, dir.magnitude, _lineOfSightLayerMask);
    }
}
