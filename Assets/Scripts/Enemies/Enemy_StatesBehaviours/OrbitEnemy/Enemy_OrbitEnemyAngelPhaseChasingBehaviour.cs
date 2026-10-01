using UnityEngine;

public class Enemy_OrbitEnemyAngelPhaseChasingBehaviour
{
    // ---------- Fases ----------
    private enum Phase { InitialWait, MoveToPosition, SecondWait, LookAtPlayer }

    // ---------- Dependencias ----------
    private readonly Transform _transform;
    private readonly Rigidbody _rb;
    private readonly Transform _playerTransform;
    private readonly FlyingEnemyStatsSO _stats;

    // ---------- Configuración ----------
    public Vector3 TargetPosition { get; set; }
    public float InitialWaitTime { get; set; } = 2f;
    public float SecondWaitTime { get; set; } = 2f;
    public float MoveSpeed { get; set; } = 5f;
    public float ArriveThreshold { get; set; } = 0.4f;
    public float LookRotationSpeed { get; set; } = 8f;
    public float BrakeSpeed { get; set; } = 6f;
    public float AimOffset { get; set; } = 1.5f;   // altura a la que mira al player

    // ---------- Estado ----------
    private Phase _currentPhase = Phase.InitialWait;
    private float _phaseTimer = 0f;


    // ---------- Constructor ----------
    public Enemy_OrbitEnemyAngelPhaseChasingBehaviour(
        Transform transform,
        Rigidbody rb,
        Transform playerTransform,
        FlyingEnemyStatsSO stats,
        Vector3 targetPosition)
    {
        _transform = transform;
        _rb = rb;
        _playerTransform = playerTransform;
        _stats = stats;
        TargetPosition = targetPosition;
    }

    // ---------- Ciclo de vida ----------
    public void EnterChase()
    {
        if (_rb != null) _rb.useGravity = false;
        _currentPhase = Phase.InitialWait;
        _phaseTimer = InitialWaitTime;
        if (_rb != null) _rb.linearVelocity = Vector3.zero;
    }

    public void ExitChase()
    {
        if (_rb != null) _rb.linearVelocity = Vector3.zero;
    }

    // ---------- Tick principal ----------
    public void Tick()
    {
        if (_playerTransform == null || _transform == null || _rb == null) return;

        switch (_currentPhase)
        {
            case Phase.InitialWait: TickInitialWait(); break;
            case Phase.MoveToPosition: TickMoveToPosition(); break;
            case Phase.SecondWait: TickSecondWait(); break;
            case Phase.LookAtPlayer: TickLookAtPlayer(); break;
        }
    }

    // ---------- Fases ----------
    private void TickInitialWait()
    {
        Brake();
        _phaseTimer -= Time.fixedDeltaTime;
        if (_phaseTimer <= 0f)
            _currentPhase = Phase.MoveToPosition;
    }

    private void TickMoveToPosition()
    {
        Vector3 toTarget = TargetPosition - _transform.position;
        float dist = toTarget.magnitude;

        if (dist <= ArriveThreshold)
        {
            _rb.linearVelocity = Vector3.zero;
            _currentPhase = Phase.SecondWait;
            _phaseTimer = SecondWaitTime;
            return;
        }

        Vector3 dir = toTarget / dist;

        _rb.linearVelocity = Vector3.Lerp(
            _rb.linearVelocity,
            dir * MoveSpeed,
            Time.fixedDeltaTime * _stats.normalReactionSpeed);

        RotateTowardsDirection(dir, _stats.rotationSpeed);
    }

    private void TickSecondWait()
    {
        Brake();
        _phaseTimer -= Time.fixedDeltaTime;
        if (_phaseTimer <= 0f)
            _currentPhase = Phase.LookAtPlayer;
    }

    private void TickLookAtPlayer()
    {
        Brake();

        Vector3 targetCenter = _playerTransform.position + Vector3.up * AimOffset;
        Vector3 dir = targetCenter - _transform.position;

        RotateTowardsDirection(dir, LookRotationSpeed);
    }

    // ---------- Helpers ----------
    private void Brake()
    {
        _rb.linearVelocity = Vector3.Lerp(
            _rb.linearVelocity,
            Vector3.zero,
            Time.fixedDeltaTime * BrakeSpeed);
    }

    private void RotateTowardsDirection(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
        _transform.rotation = Quaternion.Slerp(
            _transform.rotation, targetRot,
            Time.fixedDeltaTime * speed);
    }

    // Hook virtual por si después querés agregar ataque u otra fase
    protected virtual void ExecuteAttack() { }
}