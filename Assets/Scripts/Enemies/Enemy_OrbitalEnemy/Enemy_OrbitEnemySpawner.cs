using UnityEngine;

public class Enemy_OrbitEnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy_OrbitEnemyData _enemyData;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _amountToSpawn = 3;

    private void OnEnable()
    {
        if (_enemyData != null)
            _enemyData.OnSecondAttackMade += SpawnEnemies;
    }

    private void OnDisable()
    {
        if (_enemyData != null)
            _enemyData.OnSecondAttackMade -= SpawnEnemies;
    }

    private void SpawnEnemies()
    {
        if (_enemyPrefab == null) return;

        for (int i = 0; i < _amountToSpawn; i++)
        {
            Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);

            EnemyListMembership tracker = newEnemy.AddComponent<EnemyListMembership>();
            tracker.Initialize(_enemyData);

            _enemyData.AddEnemyAlive(newEnemy);
        }
    }
}
