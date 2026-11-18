using System.Collections.Generic;
using UnityEngine;

public class NPCOneWayWalker : MonoBehaviour
{
    private enum RouteStage
    {
        MovingToB,
        MovingToC
    }

    [Header("Waypoints")]
    [SerializeField] private List<Transform> _pointsA = new List<Transform>();
    [SerializeField] private List<Transform> _pointsB = new List<Transform>();
    [SerializeField] private List<Transform> _pointsC = new List<Transform>();

    [Header("Movement")]
    [SerializeField] private float _minMoveSpeed = 1.5f;
    [SerializeField] private float _maxMoveSpeed = 2.5f;
    [SerializeField] private float _turnSpeed = 120f;     // degrees/second
    [SerializeField] private float _acceleration = 4f;    // units/second^2
    [SerializeField] private float _arriveDistance = 0.15f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    private static readonly int _isWalkingHash = Animator.StringToHash("IsWalking");

    private RouteStage _stage;
    private Transform _currentTarget;
    private float _currentMoveSpeed;
    private Vector3 _velocity;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (!HasValidWaypoints())
            Debug.LogWarning($"{name}: requires at least 1 valid waypoint in Points A, B, and C.", this);
    }

    private void OnEnable()
    {
        if (!HasValidWaypoints())
        {
            enabled = false;
            return;
        }

        SpawnAtRandomA();
    }

    private void OnDisable()
    {
        _velocity = Vector3.zero;
        SetWalking(false);
    }

    private void Update()
    {
        if (_currentTarget == null) return;

        Vector3 toTarget = _currentTarget.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude <= _arriveDistance)
        {
            if (_stage == RouteStage.MovingToB)
            {
                _stage = RouteStage.MovingToC;
                _currentTarget = GetRandomPoint(_pointsC);
            }
            else
            {
                SpawnAtRandomA();
            }
            return;
        }

        Vector3 desiredDir = toTarget.normalized;

        if (desiredDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                _turnSpeed * Time.deltaTime
            );
        }

        Vector3 desiredVelocity = transform.forward * _currentMoveSpeed;
        _velocity = Vector3.MoveTowards(_velocity, desiredVelocity, _acceleration * Time.deltaTime);
        transform.position += _velocity * Time.deltaTime;
    }

    private void SpawnAtRandomA()
    {
        Transform pointA = GetRandomPoint(_pointsA);
        Transform pointB = GetRandomPoint(_pointsB);

        if (pointA == null || pointB == null) return;

        _stage = RouteStage.MovingToB;
        _currentTarget = pointB;
        _velocity = Vector3.zero;
        _currentMoveSpeed = Random.Range(_minMoveSpeed, Mathf.Max(_minMoveSpeed, _maxMoveSpeed));

        transform.position = pointA.position;

        Vector3 initialDir = pointB.position - pointA.position;
        initialDir.y = 0f;
        if (initialDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(initialDir);

        SetWalking(true);
    }

    private Transform GetRandomPoint(List<Transform> points)
    {
        if (points == null || points.Count == 0) return null;

        int startIndex = Random.Range(0, points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            Transform candidate = points[(startIndex + i) % points.Count];
            if (candidate != null)
                return candidate;
        }

        return null;
    }

    private bool HasValidWaypoints()
    {
        return GetRandomPoint(_pointsA) != null
            && GetRandomPoint(_pointsB) != null
            && GetRandomPoint(_pointsC) != null;
    }

    private void SetWalking(bool isWalking)
    {
        if (_animator != null)
            _animator.SetBool(_isWalkingHash, isWalking);
    }

}
