using UnityEngine;

public class Enemy_FlyingOrbitAttackBehaviour : Enemy_FlyingChasingBehaviour

{
    private Enemy_OrbitEnemyData _orbitData;
    private Enemy_OrbitEnemySecondAttackBehaviour _secondAttack;

    private bool _hasAttackedOnce = false;
    private float _secondAttackReadyTime = 0f;

    public Enemy_FlyingOrbitAttackBehaviour(Transform transform, Rigidbody rb, Transform player, Enemy_FieldOfViewBehaviour fov, Enemy_ObstacleAvoidanceBehaviour avoidance, FlyingEnemyStatsSO stats, Enemy_OrbitEnemyData orbitData)
        : base(transform, rb, player, fov, avoidance, stats)
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
        Vector3 targetCenter = _playerTransform.position + (Vector3.up * _stats.aimOffset);

        if (!_hasAttackedOnce)
        {
            _hasAttackedOnce = true;
            _secondAttack.SpawnEnemies();
            _secondAttackReadyTime = Time.time + _orbitData.SecondAttackCooldown;
            return;
        }

        bool canRollSecondAttack = Time.time >= _secondAttackReadyTime;

        if (canRollSecondAttack && Random.value <= _orbitData.SecondAttackChance)
        {
            _secondAttack.SpawnEnemies();
            _secondAttackReadyTime = Time.time + _orbitData.SecondAttackCooldown;
        }
        else
        {
            HandleOrbitWeapon(targetCenter);
        }
    }

    private void HandleOrbitWeapon(Vector3 targetPosition)
    {
        if (_orbitData.OrbitManager == null) return;

        if (_orbitData.OrbitManager.HasProjectiles)
        {
            OrbitMovement projectile = _orbitData.OrbitManager.GetNextProjectile();
            if (projectile != null)
            {
                projectile.FireAsBullet(targetPosition, _orbitData.BulletSpeed);
                Debug.Log("¡Proyectil orbital desvinculado y disparado!");
            }
        }
        else
        {
            _orbitData.InvokeRequireProjectiles();
        }
    }
}