using UnityEngine;

public class Enemy_FlyingCasterAttackBehaviour : Enemy_FlyingChasingBehaviour
{
    private Enemy_FlyingCasterEnemyData _casterData;

    public Enemy_FlyingCasterAttackBehaviour(Transform transform, Rigidbody rb, Transform player, Enemy_FieldOfViewBehaviour fov, Enemy_ObstacleAvoidanceBehaviour avoidance, FlyingEnemyStatsSO stats, Enemy_FlyingCasterEnemyData casterData) 
        : base(transform, rb, player, fov, avoidance, stats)
    {
        _casterData = casterData;
    }

    protected override void ExecuteAttack()
    {
        Vector3 targetCenter = _playerTransform.position + (Vector3.up * _stats.aimOffset);
        Vector3 shootDirection = (targetCenter - _transform.position).normalized;

        //if (_casterData != null) _casterData.SpawnProjectile(shootDirection);
        if (_casterData != null) _casterData.SpawnProjectileWithWarning(shootDirection);
    }
}
