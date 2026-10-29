
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EyeCameraController : MonoBehaviour
{
    private enum EyeCameraState
    {
        Patrol,
        Chase,
        LostTarget
    }

    [Header("References")]
    [SerializeField] private Transform _eyePivot;
    [SerializeField] private Transform _visionOrigin;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerTarget;
    [SerializeField] private float _playerHeightOffset = 1f;

    [Header("Patrol")]
    [SerializeField] private Transform[] _patrolNodes;
    [SerializeField] private float _patrolRotationSpeed = 35f;
    [SerializeField] private float _patrolWaitTime = 2f;
    [SerializeField] private float _nodeArrivalAngle = 1f;

    [Header("Tracking")]
    [SerializeField] private float _chaseRotationSpeed = 75f;
    [Range(0f, 180f)]
    [SerializeField] private float _maxTrackingAngle = 90f;

    [Header("Vision")]
    [SerializeField] private float _visionRange = 15f;
    [Range(1f, 170f)]
    [SerializeField] private float _visionAngle = 45f;
    [SerializeField] private LayerMask _obstructionMask = ~0;

    [Header("Lost Target")]
    [SerializeField] private float _lostTargetWaitTime = 3f;

    [Header("Damage")]
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] private float _damageInterval = 2f;
    private IPlayerHitable _playerHitable;

    private EyeCameraState _currentState = EyeCameraState.Patrol;

    private Quaternion _homeLocalRotation;

    private int _patrolIndex;
    private int _patrolDirection = 1;
    private bool _isWaitingAtNode;

    private float _patrolWaitTimer;
    private float _lostTargetTimer;
    private float _damageTimer;

    private IEnumerator Start()
    {
        if (_eyePivot == null)
        {
            Debug.LogError("EyeCamera is missing its EyePivot reference.", this);
            enabled = false;
            yield break;
        }

        if (_visionOrigin == null)
            _visionOrigin = _eyePivot;

        _homeLocalRotation = _eyePivot.localRotation;

        WaitForSeconds wait = new WaitForSeconds(0.25f);

        while (_player == null || _playerHitable == null)
        {
            if (_player == null && GameManager.Instance != null)
                _player = GameManager.Instance.playerModelTransform;

            if (_playerHitable == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

                if (playerObject != null)
                    _playerHitable = playerObject.GetComponent<IPlayerHitable>();
            }

            if (_player == null || _playerHitable == null)
                yield return wait;
        }
    }

    private void Update()
    {
        if (_player == null)
            return;

        bool canSeePlayer = CanSeePlayer();

        switch (_currentState)
        {
            case EyeCameraState.Patrol:

                if (canSeePlayer)
                {
                    EnterChase();
                    UpdateChase();
                }
                else
                {
                    UpdatePatrol();
                }

                break;

            case EyeCameraState.Chase:

                if (canSeePlayer)
                {
                    UpdateChase();
                }
                else
                {
                    EnterLostTarget();
                }

                break;

            case EyeCameraState.LostTarget:

                if (canSeePlayer)
                {
                    EnterChase();
                    UpdateChase();
                }
                else
                {
                    UpdateLostTarget();
                }

                break;
        }
    }

    private void UpdatePatrol()
    {
        if (_patrolNodes == null || _patrolNodes.Length == 0)
            return;

        if (_isWaitingAtNode)
        {
            _patrolWaitTimer -= Time.deltaTime;

            if (_patrolWaitTimer <= 0f)
            {
                _isWaitingAtNode = false;
                AdvancePatrolNode();
            }

            return;
        }

        Transform targetNode = _patrolNodes[_patrolIndex];

        if (targetNode == null)
        {
            AdvancePatrolNode();
            return;
        }

        Quaternion targetRotation = GetLimitedRotation(targetNode.position);

        RotateTowards(targetRotation, _patrolRotationSpeed);

        if (Quaternion.Angle(_eyePivot.rotation, targetRotation) <= _nodeArrivalAngle)
        {
            _isWaitingAtNode = true;
            _patrolWaitTimer = _patrolWaitTime;
        }
    }

    private void AdvancePatrolNode()
    {
        if (_patrolNodes.Length <= 1)
            return;

        int nextIndex = _patrolIndex + _patrolDirection;

        if (nextIndex >= _patrolNodes.Length || nextIndex < 0)
        {
            _patrolDirection *= -1;
            nextIndex = _patrolIndex + _patrolDirection;
        }

        _patrolIndex = nextIndex;
    }

    private void UpdateChase()
    {
        Vector3 targetPosition = GetPlayerTargetPosition();

        Quaternion targetRotation = GetLimitedRotation(targetPosition);

        RotateTowards(targetRotation, _chaseRotationSpeed);

        _damageTimer += Time.deltaTime;

        float interval = Mathf.Max(0.01f, _damageInterval);

        if (_damageTimer >= interval)
        {
            _damageTimer -= interval;
            DealDamage();
        }
    }

    private void UpdateLostTarget()
    {
        _lostTargetTimer -= Time.deltaTime;

        if (_lostTargetTimer <= 0f)
            EnterPatrol();
    }

    private void EnterChase()
    {
        _currentState = EyeCameraState.Chase;
        _damageTimer = _damageInterval / 2;
    }

    private void EnterLostTarget()
    {
        _currentState = EyeCameraState.LostTarget;
        _lostTargetTimer = _lostTargetWaitTime;
        _damageTimer = 0f;
    }

    private void EnterPatrol()
    {
        _currentState = EyeCameraState.Patrol;
        _isWaitingAtNode = false;
        _patrolWaitTimer = 0f;
    }

    private bool CanSeePlayer()
    {
        if (_player == null || _visionOrigin == null)
            return false;

        Vector3 origin = _visionOrigin.position;
        Vector3 target = GetPlayerTargetPosition();
        Vector3 direction = target - origin;

        float distance = direction.magnitude;

        if (distance <= 0.001f || distance > _visionRange)
            return false;

        direction /= distance;

        float angle = Vector3.Angle(_visionOrigin.forward, direction);

        if (angle > _visionAngle * 0.5f)
            return false;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, _obstructionMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == _player || hit.transform.IsChildOf(_player))
                return true;

            return false;
        }

        return true;
    }

    private Vector3 GetPlayerTargetPosition()
    {
        if (_playerTarget != null)
            return _playerTarget.position;

        return _player.position + Vector3.up * _playerHeightOffset;
    }

    private Quaternion GetLimitedRotation(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - _eyePivot.position;

        if (direction.sqrMagnitude < 0.0001f)
            return _eyePivot.rotation;

        Quaternion parentRotation = _eyePivot.parent != null
            ? _eyePivot.parent.rotation
            : Quaternion.identity;

        Quaternion homeWorldRotation = parentRotation * _homeLocalRotation;

        Quaternion offset = Quaternion.FromToRotation(
            homeWorldRotation * Vector3.forward,
            direction.normalized
        );

        Quaternion desiredWorldRotation = offset * homeWorldRotation;
        Quaternion desiredLocalRotation = Quaternion.Inverse(parentRotation) * desiredWorldRotation;

        Quaternion limitedLocalRotation = Quaternion.RotateTowards(
            _homeLocalRotation,
            desiredLocalRotation,
            _maxTrackingAngle
        );

        return parentRotation * limitedLocalRotation;
    }

    private void RotateTowards(Quaternion targetRotation, float rotationSpeed)
    {
        _eyePivot.rotation = Quaternion.RotateTowards(
            _eyePivot.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void DealDamage()
    {
        _playerHitable?.Hit(_damageAmount, gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = _visionOrigin != null ? _visionOrigin : _eyePivot;

        if (origin == null)
            return;

        Gizmos.color = Color.yellow;

        Vector3 center = origin.position + origin.forward * _visionRange;
        float radius = Mathf.Tan(_visionAngle * 0.5f * Mathf.Deg2Rad) * _visionRange;

        int segments = 24;

        for (int i = 0; i < segments; i++)
        {
            float angleA = i * Mathf.PI * 2f / segments;
            float angleB = (i + 1) * Mathf.PI * 2f / segments;

            Vector3 pointA = center + (origin.right * Mathf.Cos(angleA) + origin.up * Mathf.Sin(angleA)) * radius;
            Vector3 pointB = center + (origin.right * Mathf.Cos(angleB) + origin.up * Mathf.Sin(angleB)) * radius;

            Gizmos.DrawLine(pointA, pointB);

            if (i % 6 == 0)
                Gizmos.DrawLine(origin.position, pointA);
        }

        if (_patrolNodes == null)
            return;

        Gizmos.color = Color.cyan;

        foreach (Transform node in _patrolNodes)
        {
            if (node == null)
                continue;

            Gizmos.DrawWireSphere(node.position, 0.2f);
            Gizmos.DrawLine(_eyePivot.position, node.position);
        }
    }
}
