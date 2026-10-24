using UnityEngine;

public class Enemy_OrbitEnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy_OrbitEnemyData _enemyData;
    [SerializeField] private Transform _spawnPoint;

    [Header("Participación (opt-in por evento)")]
    [Tooltip("Si true, este spawner instancia en la wave 1 de la fase 2.")]
    [SerializeField] private bool _spawnOnPhase2Wave1;
    [Tooltip("Si true, este spawner instancia en los respawns de wave 2 o más.")]
    [SerializeField] private bool _spawnOnPhase2Wave2Plus;

    [Header("Fase 1 (solo si ambos bools son false)")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _amountToSpawn = 3;

    [Header("Fase 2 – Wave 1 (solo si _spawnOnPhase2Wave1)")]
    [SerializeField] private GameObject _phase2Wave1Prefab;
    [SerializeField] private int _phase2Wave1Amount = 3;

    [Header("Fase 2 – Wave 2+ (solo si _spawnOnPhase2Wave2Plus)")]
    [SerializeField] private GameObject _phase2Wave2PlusPrefab;
    [SerializeField] private int _phase2Wave2PlusAmount = 3;
    [Tooltip("Si true, los enemigos de wave 2+ retienen la barrera hasta morir.")]
    [SerializeField] private bool _phase2Wave2PlusCountsAsAlive = true;

    private void OnEnable()
    {
        if (_enemyData == null) return;
        _enemyData.OnSecondAttackMade += HandlePhase1SecondAttack;
        _enemyData.OnSecondPhaseSpawnRequested += HandlePhase2Wave1;
        _enemyData.OnSecondPhaseRespawnRequested += HandlePhase2Wave2Plus;
    }

    private void OnDisable()
    {
        if (_enemyData == null) return;
        _enemyData.OnSecondAttackMade -= HandlePhase1SecondAttack;
        _enemyData.OnSecondPhaseSpawnRequested -= HandlePhase2Wave1;
        _enemyData.OnSecondPhaseRespawnRequested -= HandlePhase2Wave2Plus;
    }

    private void HandlePhase1SecondAttack()
    {
        // Ambos bools true/false en fase-2 → no participa en fase 1
        if (_spawnOnPhase2Wave1 || _spawnOnPhase2Wave2Plus) return;
        SpawnEnemies(_enemyPrefab, _amountToSpawn, trackAsAlive: true);
    }

    private void HandlePhase2Wave1()
    {
        if (!_spawnOnPhase2Wave1) return;
        SpawnEnemies(_phase2Wave1Prefab, _phase2Wave1Amount, trackAsAlive: true);
    }

    private void HandlePhase2Wave2Plus()
    {
        if (!_spawnOnPhase2Wave2Plus) return;
        SpawnEnemies(_phase2Wave2PlusPrefab, _phase2Wave2PlusAmount,
                     trackAsAlive: _phase2Wave2PlusCountsAsAlive);
    }

    private void SpawnEnemies(GameObject prefab, int amount, bool trackAsAlive)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[OrbitEnemySpawner] Prefab no asignado para el evento actual.", this);
            return;
        }

        Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;

        for (int i = 0; i < amount; i++)
        {
            GameObject newEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);

            if (trackAsAlive)
            {
                var tracker = newEnemy.AddComponent<EnemyListMembership>();
                tracker.Initialize(_enemyData);
                _enemyData.AddEnemyAlive(newEnemy);
            }
        }
    }
}
