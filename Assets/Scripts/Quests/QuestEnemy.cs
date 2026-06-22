using UnityEngine;

public class QuestEnemy : MonoBehaviour
{
    [Tooltip("Unique ID matching the enemyID in QuestKillCondition")]
    [SerializeField] private string enemyID;

    private Enemy_HealthSystem _healthSystem;
    private bool _isDead = false;

    private void Start()
    {
        _healthSystem = GetComponent<Enemy_HealthSystem>();

        if (_healthSystem == null)
        {
            Debug.LogWarning($"[QuestEnemy] No Enemy_HealthSystem found on {gameObject.name}");
            return;
        }

        _healthSystem.OnDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        if (_isDead) return;
        _isDead = true;
        QuestManager.Instance.RegisterKill(enemyID);
    }

    private void OnDestroy()
    {
        if (_healthSystem != null)
            _healthSystem.OnDeath -= OnEnemyDeath;
    }
}