using UnityEngine;

public class Enemy_FlyingOrbitAttackBehaviour : Enemy_FlyingChasingBehaviour

{
    private Enemy_OrbitEnemyData _orbitData;

    public Enemy_FlyingOrbitAttackBehaviour(Transform transform, Rigidbody rb, Transform player, Enemy_FieldOfViewBehaviour fov, Enemy_ObstacleAvoidanceBehaviour avoidance, FlyingEnemyStatsSO stats, Enemy_OrbitEnemyData orbitData) 
        : base(transform, rb, player, fov, avoidance, stats)
    {
        _orbitData = orbitData;
    }

    protected override void ExecuteAttack()
    {
        Vector3 targetCenter = _playerTransform.position + (Vector3.up * _stats.aimOffset);
        
        // Antes pasábamos un shootDirection, ahora le pasamos la POSICIÓN directa
        HandleOrbitWeapon(targetCenter); 
    }

    // 2. Modificamos el parámetro que recibe y envía
    private void HandleOrbitWeapon(Vector3 targetPosition) 
    {
        if (_orbitData.OrbitManager == null) return;

        if (_orbitData.OrbitManager.HasProjectiles)
        {
            OrbitMovement projectile = _orbitData.OrbitManager.GetNextProjectile();
            if (projectile != null)
            {
                // Disparamos usando la Posición exacta
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