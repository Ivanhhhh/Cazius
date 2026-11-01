using UnityEngine;
using UnityEngine.VFX;
using System;
using System.Collections.Generic;


public class Enemy_OrbitEnemyAngelPhaseChasingBehaviour
{
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
    protected float _lastAttackTime = -100f;

    // Usamos Time.time (tiempo absoluto) para evitar que los timers varíen según los FPS.
    private float _phaseEndTime;
    private readonly List<(float Time, Action Action)> _timeline = new List<(float, Action)>();

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

    // Agenda una acción guardando el momento exacto en que debe ocurrir (Time.time + segundos)
    protected void ScheduleInPhase(float seconds, Action action)
    {
        _timeline.Add((Time.time + seconds, action));
    }

    // Recorre la lista hacia atrás para poder eliminar elementos mientras la recorres de forma segura
    private void UpdateTimeline()
    {
        // Recorremos HACIA ADELANTE para respetar el orden de programación
        for (int i = 0; i < _timeline.Count; i++)
        {
            if (Time.time >= _timeline[i].Time)
            {
                Action action = _timeline[i].Action;
                _timeline.RemoveAt(i);
                i--; // Importante: al remover, el siguiente elemento se mueve a este índice, así que retrocedemos i

                action(); // Ejecutamos la acción
            }
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
        _timeline.Clear();

        switch (newPhase)
        {
            case Phase.InitialWait:
                // Guardamos la hora exacta en la que termina la espera
                _phaseEndTime = Time.time + InitialWaitTime;
                OnInitialWaitEntered();
                break;
            case Phase.SecondWait:
                _phaseEndTime = Time.time + SecondWaitTime;
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

        // Comparamos el tiempo actual con la hora de finalización
        if (Time.time >= _phaseEndTime)
        {
            Debug.Log("FirstWaitFinished");
            SetPhase(Phase.MoveToPosition);
        }
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

        // Usamos Time.deltaTime para movimiento suave en Update
        _rb.linearVelocity = Vector3.Lerp(
            _rb.linearVelocity,
            dir * MoveSpeed,
            Time.deltaTime * _stats.normalReactionSpeed);

        RotateTowardsDirection(dir, _stats.rotationSpeed);
    }

    private void TickSecondWait()
    {
        Brake();
        UpdateTimeline();
        LookAtPlayer();

        if (Time.time >= _phaseEndTime)
        {
            SetPhase(Phase.LookAtPlayer);
            Debug.Log("Second wait finished");
        }
    }

    private void TickLookAtPlayer()
    {
        Brake();

        Vector3 targetCenter = _playerTransform.position + Vector3.up * AimOffset;
        Vector3 dir = targetCenter - _transform.position;

        RotateTowardsDirection(dir, LookRotationSpeed);

        // Cooldown basado en tiempo absoluto (Time.time)
        if (Time.time >= _lastAttackTime + _stats.shootCooldown)
        {
            _lastAttackTime = Time.time;
            ExecuteAttack();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 targetCenter = _playerTransform.position + Vector3.up * AimOffset;
        Vector3 dir = targetCenter - _transform.position;
        RotateTowardsDirection(dir, LookRotationSpeed);
    }

    // ---------- Helpers ----------
    protected void Brake()
    {
        // Time.deltaTime para que el frenado sea consistente sin importar los FPS
        _rb.linearVelocity = Vector3.Lerp(
            _rb.linearVelocity,
            Vector3.zero,
            Time.deltaTime * BrakeSpeed);
    }

    protected void RotateTowardsDirection(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);

        // Time.deltaTime para que la rotación se vea fluida en Update
        _transform.rotation = Quaternion.Slerp(
            _transform.rotation, targetRot,
            Time.deltaTime * speed);
    }

    protected virtual void ExecuteAttack() { }
    protected virtual void OnInitialWaitEntered() { }
    protected virtual void OnSecondWaitEntered() { }
}