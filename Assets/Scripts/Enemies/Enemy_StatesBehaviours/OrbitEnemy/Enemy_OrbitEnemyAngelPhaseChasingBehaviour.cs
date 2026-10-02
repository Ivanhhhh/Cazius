using UnityEngine;
using UnityEngine.VFX;
using System;
using System.Collections.Generic;


public class Enemy_OrbitEnemyAngelPhaseChasingBehaviour
{
    private struct TimedAction
    {
        public float Time;
        public Action Action;
    }
    private enum Phase { InitialWait, MoveToPosition, SecondWait, LookAtPlayer }

    // ---------- Dependencias ----------
    protected readonly Transform _transform;
    protected readonly Rigidbody _rb;
    protected readonly Transform _playerTransform;
    protected readonly FlyingEnemyStatsSO _stats;

    // ---------- Configuración ----------
    public Vector3 TargetPosition { get; set; }
    public float InitialWaitTime { get; set; } = 2f;
    public float SecondWaitTime { get; set; } = 2f;
    public float MoveSpeed { get; set; } = 5f;
    public float ArriveThreshold { get; set; } = 0.4f;
    public float LookRotationSpeed { get; set; } = 8f;
    public float BrakeSpeed { get; set; } = 6f;
    public float AimOffset { get; set; } = 1.5f;

    // ---------- Estado ----------
    private Phase _currentPhase = Phase.InitialWait;
    private float _phaseTimer = 0f;
    protected float _lastAttackTime = -100f;

    private readonly List<TimedAction> _timeline = new List<TimedAction>();
    private float _phaseElapsed;

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

    protected void ScheduleInPhase(float seconds, Action action)
    {
        _timeline.Add(new TimedAction { Time = seconds, Action = action });
    }

    private void UpdateTimeline()
    {
        _phaseElapsed += Time.fixedDeltaTime;

        for (int i = 0; i < _timeline.Count;)
        {
            if (_phaseElapsed >= _timeline[i].Time)
            {
                Action action = _timeline[i].Action;
                _timeline.RemoveAt(i);
                action();
            }
            else i++;
        }
    }

    // ---------- Ciclo de vida ----------
    public virtual void EnterChase()
    {
        if (_rb != null) _rb.useGravity = false;
        _lastAttackTime = -100f;
        if (_rb != null) _rb.linearVelocity = Vector3.zero;
        SetPhase(Phase.InitialWait);
    }

    private void SetPhase(Phase newPhase)
    {
        _currentPhase = newPhase;
        _phaseElapsed = 0f;
        _timeline.Clear();   // lo programado en la fase anterior no sobrevive

        switch (newPhase)
        {
            case Phase.InitialWait:
                _phaseTimer = InitialWaitTime;
                OnInitialWaitEntered();
                break;
            case Phase.SecondWait:
                _phaseTimer = SecondWaitTime;
                OnSecondWaitEntered();
                break;
        }
    }


    public virtual void ExitChase()
    {
        if (_rb != null) _rb.linearVelocity = Vector3.zero;
    }

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
        UpdateTimeline();
        _phaseTimer -= Time.fixedDeltaTime;
        if (_phaseTimer <= 0f)
            SetPhase(Phase.MoveToPosition);
    }

    private void TickMoveToPosition()
    {
        Vector3 toTarget = TargetPosition - _transform.position;
        float dist = toTarget.magnitude;

        if (dist <= ArriveThreshold)
        {
            _rb.linearVelocity = Vector3.zero;
            SetPhase(Phase.SecondWait);
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
        UpdateTimeline();
        _phaseTimer -= Time.fixedDeltaTime;
        if (_phaseTimer <= 0f)
            SetPhase(Phase.LookAtPlayer);
    }

    private void TickLookAtPlayer()
    {
        Brake();

        Vector3 targetCenter = _playerTransform.position + Vector3.up * AimOffset;
        Vector3 dir = targetCenter - _transform.position;

        RotateTowardsDirection(dir, LookRotationSpeed);

        // La base maneja el cooldown; el hijo solo decide QUÉ atacar.
        if (Time.time >= _lastAttackTime + _stats.shootCooldown)
        {
            _lastAttackTime = Time.time;
            ExecuteAttack();
        }
    }

    // ---------- Helpers ----------
    protected void Brake()
    {
        _rb.linearVelocity = Vector3.Lerp(
            _rb.linearVelocity,
            Vector3.zero,
            Time.fixedDeltaTime * BrakeSpeed);
    }

    protected void RotateTowardsDirection(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
        _transform.rotation = Quaternion.Slerp(
            _transform.rotation, targetRot,
            Time.fixedDeltaTime * speed);
    }

    // Hook para los hijos
    protected virtual void ExecuteAttack() { }
    protected virtual void OnInitialWaitEntered() { }
    protected virtual void OnSecondWaitEntered() { }
}