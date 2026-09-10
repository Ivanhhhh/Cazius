using UnityEngine;

public class Enemy_SpeedBoostBehaviour : MonoBehaviour
{
    private readonly Transform _selfTransform;
    private readonly Transform _playerTransform;
    private readonly Enemy_MeleeEnemy_Data _enemyData;
    private readonly float _distanceThreshold;
    private readonly float _movementSpeedMultiplier;
    private readonly Animator _animator; // Opcional, para ajustar la velocidad de animación
    private readonly float _animSpeedMultiplier;

    private bool _isBoosted;

    public Enemy_SpeedBoostBehaviour(
        Transform selfTransform,
        Transform playerTransform,
        Enemy_MeleeEnemy_Data enemyData,
        float distanceThreshold,
        float movementSpeedMultiplier,
        Animator animator = null,
        float animSpeedMultiplier = 1f)
    {
        _selfTransform = selfTransform;
        _playerTransform = playerTransform;
        _enemyData = enemyData;
        _distanceThreshold = distanceThreshold;
        _movementSpeedMultiplier = movementSpeedMultiplier;
        _animator = animator;
        _animSpeedMultiplier = animSpeedMultiplier;
    }

    public void Tick()
    {
        if (_playerTransform == null || _enemyData == null)
            return;

        float distance = Vector3.Distance(_selfTransform.position, _playerTransform.position);
        bool shouldBoost = distance > _distanceThreshold;

        if (shouldBoost == _isBoosted)
            return;

        _isBoosted = shouldBoost;

        if (_isBoosted)
        {
            // Aplicar multiplicador de velocidad
            _enemyData.AddSpeedMultiplier(this, _movementSpeedMultiplier);

            // Opcional: acelerar la animación
            if (_animator != null)
                _animator.speed = _animSpeedMultiplier;

            Debug.Log("Speed boost activado");
        }
        else
        {
            // Quitar multiplicador
            _enemyData.RemoveSpeedMultiplier(this);

            // Restaurar animación
            if (_animator != null)
                _animator.speed = 1f;

            Debug.Log("Speed boost desactivado");
        }
    }
}