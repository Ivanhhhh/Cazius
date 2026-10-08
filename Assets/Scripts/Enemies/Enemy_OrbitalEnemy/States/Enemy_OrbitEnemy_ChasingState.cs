using UnityEngine;

public class Enemy_OrbitEnemy_ChasingState : Enemy_Interface_StateMachine
{
    Enemy_OrbitEnemy_StateMachine _stateMachine;
    Enemy_OrbitEnemyData _data;


    [Range(0f, 1f)]
    private const float SecondPhaseHealthThreshold = 0.5f;

    public Enemy_OrbitEnemy_ChasingState(Enemy_OrbitEnemy_StateMachine stateMachine, Enemy_OrbitEnemyData data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }

    public void OnEnter()
    {
        _data._inSecondPhase = false;
        _data._chasing.EnterChase();
    }

    public void OnExit()
    {
        // Cierra el behaviour que esté activo en este momento
        if (_data._inSecondPhase)
            _data._angelPhaseChasing.ExitChase();
        else
            _data._chasing.ExitChase();
    }

    public void OnUpdate()
    {
        // 1) Chequeo de transición a segunda fase
        if (!_data._inSecondPhase && IsAtOrBelowHalfHealth())
        {
            // Dispara todo lo que está orbitando (no bloquea, no espera)
            _data.FireAllOrbitProjectiles();

            _data._chasing.ExitChase();
            _data._angelPhaseChasing.EnterChase();
            _data._inSecondPhase = true;
        }

        // 2) Tick del behaviour activo
        if (_data._inSecondPhase)
            _data._angelPhaseChasing.Tick();
        else
            _data._chasing.Tick();
    }

    // ---------- Helper ----------
    private bool IsAtOrBelowHalfHealth()
    {
        var hs = _data.HealthSystem;
        if (hs == null) return false;

        // Ajustá el cálculo según tu Enemy_HealthSystem_Base
        return hs.CurrentHealth <= hs.MaxHealth * SecondPhaseHealthThreshold;
    }
}
