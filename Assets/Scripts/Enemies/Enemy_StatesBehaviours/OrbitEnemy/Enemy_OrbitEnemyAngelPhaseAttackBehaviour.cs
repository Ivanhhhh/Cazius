using UnityEngine;

public class Enemy_OrbitEnemyAngelPhaseAttackBehaviour : Enemy_OrbitEnemyAngelPhaseChasingBehaviour
{
    private readonly Enemy_OrbitEnemyData _orbitData;
    private readonly Enemy_OrbitEnemySecondAttackBehaviour _secondAttack;

    private bool _hasAttackedOnce;
    private float _secondAttackReadyTime;

    public Enemy_OrbitEnemyAngelPhaseAttackBehaviour(
        Transform transform,
        Rigidbody rb,
        Transform playerTransform,
        FlyingEnemyStatsSO stats,
        Vector3 targetPosition,
        Enemy_OrbitEnemyData orbitData)
        : base(transform, rb, playerTransform, stats, targetPosition)
    {
        _orbitData = orbitData;
        _secondAttack = new Enemy_OrbitEnemySecondAttackBehaviour(orbitData);
    }

    public override void EnterChase()
    {
        base.EnterChase();
        _hasAttackedOnce = false;
        _secondAttackReadyTime = 0f;
    }

    protected override void ExecuteAttack()
    {
        if (!_hasAttackedOnce)
        {
            _hasAttackedOnce = true;
            DoSecondAttack();
            return;
        }

        bool canRollSecondAttack = Time.time >= _secondAttackReadyTime;

        if (canRollSecondAttack && Random.value <= _orbitData.SecondAttackChance)
            DoSecondAttack();
        else
            FireMeteorites();
    }

    private void DoSecondAttack()
    {
        _secondAttack.SpawnEnemies();
        _secondAttackReadyTime = Time.time + _orbitData.SecondAttackCooldown;
    }

    private void FireMeteorites()
    {
        Enemy_MeteoriteSpawnZone zone = _orbitData.EnemyMeteoriteSpawnZone;
        if (zone == null) return;

        zone.SpawnMultiple(_orbitData.MeteoriteCount);
    }
    protected override void OnInitialWaitEntered()
    {
        if (_orbitData.AngelBossVisuals != null)
            _orbitData.AngelBossVisuals.EyeChangePhase();
            _orbitData.HealthSystem.SetInvulnerable(true);
    }

    protected override void OnSecondWaitEntered()
    {
        ScheduleInPhase(_orbitData.ModelSwapTime, SwapModels);
        ScheduleInPhase(_orbitData.ThroneSpawnTime, SpawnThrone);
        ScheduleInPhase(_orbitData.LaserPlayTime, PlayLaser);
        _orbitData.HealthSystem.SetInvulnerable(false);
    }

    private void SwapModels()
    {
        if (_orbitData.FirstPhaseModel != null) _orbitData.FirstPhaseModel.SetActive(false);
        if (_orbitData.SecondPhaseModel != null) _orbitData.SecondPhaseModel.SetActive(true);
    }

    private void SpawnThrone()
    {
        if (_orbitData.ThroneVisuals != null) _orbitData.ThroneVisuals.Spawn();
    }

    private void PlayLaser()
    {
        if (_orbitData.LaserBeamEffect != null) _orbitData.LaserBeamEffect.SetActive(true);
    }
}