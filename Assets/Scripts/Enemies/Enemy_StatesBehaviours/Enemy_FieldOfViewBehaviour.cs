using UnityEngine;

public class Enemy_FieldOfViewBehaviour
{
    private Transform _playerTransform;
    private float _radiusVision;
    private float _angleVision;
    private Transform _objectTransform;
    private LayerMask _lineOfSightLayerMask;
    public Enemy_FieldOfViewBehaviour(Transform playerTransform, float radiusVision, Transform objectTransform, float angleVision, LayerMask lineOfSightLayerMask)
    {
        _playerTransform = playerTransform;
        _radiusVision = radiusVision;
        _objectTransform = objectTransform;
        _angleVision = angleVision;
        _lineOfSightLayerMask = lineOfSightLayerMask;
    }

    public bool CanseePlayer()
    {
        if (_playerTransform == null) return false;
        return InFieldOfView(_playerTransform.position);
    }
    bool InFieldOfView(Vector3 endPos)
    {
        Vector3 dir = endPos - _objectTransform.position;
        if (dir.magnitude > _radiusVision) return false;
        if (Vector3.Angle(_objectTransform.forward, dir) > _angleVision / 2) return false;
        if (!InLineOfSight(_objectTransform.position, endPos)) return false;
        return true;
    }
    bool InLineOfSight(Vector3 startPos, Vector3 endPos)
    {
        Vector3 dir = endPos - startPos;
        return !Physics.Raycast(startPos,dir.normalized,dir.magnitude,_lineOfSightLayerMask);
    }
}
