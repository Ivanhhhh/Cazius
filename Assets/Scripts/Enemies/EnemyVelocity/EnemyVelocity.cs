using UnityEngine;

public class EnemyVelocity : MonoBehaviour
{
    //[SerializeField] float Distance;

    [Header("Distance Settings")]
    [Tooltip("The distance threshold. If the player is further than this, the enemy speeds up.")]
    [SerializeField] private float distanceThreshold;

    [Header("Multipliers")]
    [SerializeField] private float animSpeedMultiplier = 2f;
    [SerializeField] private float movementSpeedMultiplier = 2f;

    [Header("Base Values")]
    [SerializeField] private float baseMovementSpeed = 3.5f;

    [SerializeField] Animator enemyAnimator;
    private float currentMovementSpeed;

    // Cache the hash for performance rather than using string lookups every frame
    //private static readonly int AnimSpeedHash = Animator.StringToHash("AnimSpeed");

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        currentMovementSpeed = baseMovementSpeed; // la velocidad actual es la
    }

    void Update()
    {
        // Ensure the player is registered before running distance checks
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            CheckDistanceToPlayer();
        }
    }

  private void CheckDistanceToPlayer()
    {
        // 1. Calculate the distance to the player using your GameManager instance
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);

        // 2. Check if the player is further than the parameterized threshold
        if (distanceToPlayer > distanceThreshold)
        {
            // Apply multipliers
            currentMovementSpeed = baseMovementSpeed * movementSpeedMultiplier;
            //enemyAnimator.SetFloat(AnimSpeedHash, animSpeedMultiplier);
            enemyAnimator.speed = 2f;

            Debug.LogWarning("velocidadAumentada");
        }
        else
        {
            // Revert back to normal values
            currentMovementSpeed = baseMovementSpeed;
            //enemyAnimator.SetFloat(AnimSpeedHash, 1f); // 1f is normal default speed
            enemyAnimator.speed = 1f;
            Debug.LogWarning("velocidadNormal");
        }

    }
}
