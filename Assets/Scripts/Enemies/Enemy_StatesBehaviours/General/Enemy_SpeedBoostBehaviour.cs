using UnityEngine;

public class Enemy_SpeedBoostBehaviour : MonoBehaviour
{
    private readonly Transform _selfTransform;
    private readonly Transform _playerTransform;
    private readonly Enemy_MeleeEnemy_Data _enemyData;

    // Umbral LEJANO: si se aleja más que esto → activa boost
    private readonly float _activateDistance;

    // Umbral CERCANO: si estando boosteado se acerca menos que esto → quita boost
    private readonly float _deactivateDistance;

    private readonly float _movementSpeedMultiplier;
    private readonly Animator _animator;
    private readonly float _animSpeedMultiplier;

    private bool _isBoosted;
    public bool IsCurrentlyBoosted => _isBoosted;

    public Enemy_SpeedBoostBehaviour(
        Transform selfTransform,
        Transform playerTransform,
        Enemy_MeleeEnemy_Data enemyData,
        float activateDistance,
        float deactivateDistance,
        float movementSpeedMultiplier,
        Animator animator = null,
        float animSpeedMultiplier = 1f)
    {
        _selfTransform = selfTransform;
        _playerTransform = playerTransform;
        _enemyData = enemyData;
        _activateDistance = activateDistance;
        _deactivateDistance = Mathf.Min(deactivateDistance, activateDistance);
        _movementSpeedMultiplier = movementSpeedMultiplier;
        _animator = animator;
        _animSpeedMultiplier = animSpeedMultiplier;
    }

    public void Tick()
    {
        if (_playerTransform == null || _enemyData == null)
            return;

        float distance = Vector3.Distance(_selfTransform.position, _playerTransform.position);

        // ---- Histéresis ----
        bool shouldBoost;
        if (_isBoosted)
            shouldBoost = distance > _deactivateDistance;
        else
            shouldBoost = distance > _activateDistance;

        if (shouldBoost == _isBoosted)
            return;

        _isBoosted = shouldBoost;

        if (_isBoosted)
        {
            // 1) multiplicador de velocidad
            _enemyData.AddSpeedMultiplier(this, _movementSpeedMultiplier);

            // 2) flag local + limpieza de slows + evento
            //    (SetBoostActive hace el RecalculateSpeed al final)
            _enemyData.SetBoostActive(true);

            // 3) animación
            if (_animator != null)
            {
                _animator.speed = _animSpeedMultiplier;
                _animator.SetBool("IsFlying", true);
            }

            Debug.Log($"[SpeedBoost] ON  (dist={distance:F2}, activa>{_activateDistance})");
        }
        else
        {
            _enemyData.RemoveSpeedMultiplier(this);
            _enemyData.SetBoostActive(false);   // ← ya recalcula

            if (_animator != null)
            {
                _animator.speed = 1f;
                _animator.SetBool("IsFlying", false);
            }

            Debug.Log($"[SpeedBoost] OFF (dist={distance:F2}, desactiva<{_deactivateDistance})");
        }
    }

}