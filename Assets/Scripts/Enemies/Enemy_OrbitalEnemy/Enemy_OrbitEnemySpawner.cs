using UnityEngine;

public class Enemy_OrbitEnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy_OrbitEnemyData _enemyData;
    [SerializeField] private Transform _spawnPoint;

    [Header("Fase 1")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _amountToSpawn = 3;

    [Header("Fase 2")]
    [SerializeField] private GameObject _secondPhasePrefab;
    [SerializeField] private int _secondPhaseAmountToSpawn = 3;
    [Tooltip("Si está activo, en fase 2 NO se spawnea con OnSecondAttackMade. Solo cuando el Data llama a RequestSecondPhaseSpawn().")]
    [SerializeField] private bool _secondPhaseOnlyOnRequest = false;

    private bool _oneTime = false;

    private void OnEnable()
    {
        if (_enemyData == null) return;
        if (_oneTime) { return; }
        _enemyData.OnSecondAttackMade += HandleSecondAttackMade;
        _enemyData.OnSecondPhaseSpawnRequested += SpawnSecondPhaseEnemies;
        _oneTime = true;
    }

    private void OnDestroy()
    {
        if (_enemyData == null) return;
        _enemyData.OnSecondAttackMade -= HandleSecondAttackMade;
        _enemyData.OnSecondPhaseSpawnRequested -= SpawnSecondPhaseEnemies;
    }

    // Se ejecuta con el evento existente: decide qué spawnear según la fase.
    private void HandleSecondAttackMade()
    {
        if (_enemyData.InSecondPhase)
        {
            SpawnSecondPhaseEnemies();
        }
        else
        {
            if (_secondPhaseOnlyOnRequest) return; // lo maneja RequestSecondPhaseSpawn

            SpawnEnemies(_enemyPrefab, _amountToSpawn);
        }
    }

    // Spawn de fase 2: lo llama el evento del Data o el handler de arriba.
    public void SpawnSecondPhaseEnemies()
    {
        SpawnEnemies(_secondPhasePrefab, _secondPhaseAmountToSpawn);
    }

    private void SpawnEnemies(GameObject prefab, int amount)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[OrbitEnemySpawner] Falta el prefab para esta fase.", this);
            return;
        }

        Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;

        for (int i = 0; i < amount; i++)
        {
            GameObject newEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);

            EnemyListMembership tracker = newEnemy.AddComponent<EnemyListMembership>();
            tracker.Initialize(_enemyData);

            _enemyData.AddEnemyAlive(newEnemy);
        }
    }
}
