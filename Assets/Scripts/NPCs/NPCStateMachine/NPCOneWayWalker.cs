using System.Collections;
using UnityEngine;

public class NPCOneWayWalker : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _turnSpeed = 120f;     // degrees/second
    [SerializeField] private float _acceleration = 4f;    // units/second^2
    [SerializeField] private float _arriveDistance = 0.15f;

    [Header("Respawn Delay at Point B")]
    [SerializeField] private float _minRespawnDelay = 2f;
    [SerializeField] private float _maxRespawnDelay = 6f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    private static readonly int _isWalkingHash = Animator.StringToHash("IsWalking");

    private Vector3 _velocity;
    private bool _isWaitingToRespawn;
    private Coroutine _respawnCoroutine;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (_pointA == null || _pointB == null)
            Debug.LogWarning($"{name}: requires both Point A and Point B assigned.", this);
    }

    private void OnEnable()
    {
        if (_pointA == null || _pointB == null)
        {
            enabled = false;
            return;
        }

        SpawnAtPointA();
    }

    private void OnDisable()
    {
        if (_respawnCoroutine != null)
        {
            StopCoroutine(_respawnCoroutine);
            _respawnCoroutine = null;
        }

        _isWaitingToRespawn = false;
        _velocity = Vector3.zero;
        SetWalking(false);
    }

    private void Update()
    {
        if (_isWaitingToRespawn) return;

        Vector3 toTarget = _pointB.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude <= _arriveDistance)
        {
            _respawnCoroutine = StartCoroutine(WaitAndRespawnRoutine());
            return;
        }

        Vector3 desiredDir = toTarget.normalized;

        // Smooth rotation toward Point B
        if (desiredDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                _turnSpeed * Time.deltaTime
            );
        }

        // Smooth acceleration forward
        Vector3 desiredVelocity = transform.forward * _moveSpeed;
        _velocity = Vector3.MoveTowards(_velocity, desiredVelocity, _acceleration * Time.deltaTime);
        transform.position += _velocity * Time.deltaTime;
    }

    private IEnumerator WaitAndRespawnRoutine()
    {
        _isWaitingToRespawn = true;
        _velocity = Vector3.zero;
        SetWalking(false);

        float delay = Random.Range(_minRespawnDelay, Mathf.Max(_minRespawnDelay, _maxRespawnDelay));
        yield return new WaitForSeconds(delay);

        SpawnAtPointA();
        _respawnCoroutine = null;
    }

    private void SpawnAtPointA()
    {
        _isWaitingToRespawn = false;
        _velocity = Vector3.zero;

        transform.position = _pointA.position;

        Vector3 initialDir = _pointB.position - _pointA.position;
        initialDir.y = 0f;
        if (initialDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(initialDir);

        SetWalking(true);
    }

    private void SetWalking(bool isWalking)
    {
        if (_animator != null)
            _animator.SetBool(_isWalkingHash, isWalking);
    }

}
