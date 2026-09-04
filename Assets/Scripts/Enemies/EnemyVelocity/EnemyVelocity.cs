using UnityEngine;

public class EnemyVelocity : MonoBehaviour
{
    [Header("Distance Settings")]
    [SerializeField] private float distanceThreshold;

    [Header("Multipliers")]
    [SerializeField] private float animSpeedMultiplier = 2f;
    [SerializeField] private float movementSpeedMultiplier = 2f;

    [Header("References")]
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Enemy_MeleeEnemy_Data enemyData;

    private float baseMovementSpeed;

    private bool _isSpeedBoosted;

    private void Start()
    {
        if (enemyAnimator == null)
            enemyAnimator = GetComponent<Animator>();

        if (enemyData == null)
            enemyData = GetComponent<Enemy_MeleeEnemy_Data>();

        // Save the ORIGINAL chase speed
        baseMovementSpeed = enemyData.GetChaseSpeed();
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.Player == null)
            return;

        CheckDistanceToPlayer();
    }

    private void CheckDistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(
            transform.position,
            GameManager.Instance.Player.transform.position
        );

        bool shouldBoost = distanceToPlayer > distanceThreshold;

        if (shouldBoost == _isSpeedBoosted)
            return;

        _isSpeedBoosted = shouldBoost;

        if (_isSpeedBoosted)
        {
            float boostedSpeed =
                baseMovementSpeed * movementSpeedMultiplier;

            enemyData.SetChaseSpeed(boostedSpeed);

            enemyAnimator.speed = animSpeedMultiplier;

            Debug.Log("Enemy speed increased");
        }
        else
        {
            enemyData.SetChaseSpeed(baseMovementSpeed);

            enemyAnimator.speed = 1f;

            Debug.Log("Enemy speed normal");
        }
    }
}