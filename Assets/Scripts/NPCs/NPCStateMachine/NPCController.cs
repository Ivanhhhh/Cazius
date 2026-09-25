using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private List<Transform> _waypoints = new List<Transform>();

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _turnSpeed = 120f; // degrees/second
    [SerializeField] private float _acceleration = 4f; // units/second^2, controls turn radius
    [SerializeField] private float _arriveDistance = 0.1f;

    [Header("Idle")]
    [SerializeField] private float _waitTime = 2f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    private static readonly int _isWalkingHash = Animator.StringToHash("IsWalking");

    private int _waypointIndex;
    private int _direction = 1; // 1 = forward, -1 = backward

    public NPCStateMachine Machine { get; private set; }
    public NPCIdleState IdleState { get; private set; }
    public NPCWalkingState WalkingState { get; private set; }
    public NPCTalkingState TalkingState { get; private set; }

    public Animator Animator => _animator;

    public float MoveSpeed => _moveSpeed;
    public float TurnSpeed => _turnSpeed;
    public float Acceleration => _acceleration;
    public float ArriveDistance => _arriveDistance;
    public float WaitTime => _waitTime;
    public Transform CurrentWaypoint => _waypoints[_waypointIndex];

    // Persists across state changes
    public Vector3 Velocity { get; set; }

    private void Awake()
    {
        Machine = new NPCStateMachine();
        IdleState = new NPCIdleState(this);
        WalkingState = new NPCWalkingState(this);
        TalkingState = new NPCTalkingState(this);

        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    public void SetWalking(bool isWalking)
    {
        if (_animator != null)
            _animator.SetBool(_isWalkingHash, isWalking);
    }

    private void Start()
    {
        if (_waypoints.Count < 2)
        {
            Debug.LogWarning($"{name}: needs at least 2 Waypoints.", this);
            enabled = false;
            return;
        }

        Machine.ChangeState(IdleState);
    }

    private void Update()
    {
        Machine.Tick();
    }

    // Reverse direction at either end
    public void AdvanceWaypoint()
    {
        int next = _waypointIndex + _direction;

        if (next >= _waypoints.Count || next < 0)
        {
            _direction *= -1;
            next = _waypointIndex + _direction;
        }

        _waypointIndex = next;
    }

}
