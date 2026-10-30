using UnityEngine;
using FactoryPool;

public class Enemy_SpawnOnImpact : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("El prefab o ID del pool a spawnear (ej. moneda, alma, partículas).")]
    [SerializeField] private string _poolIdToSpawn; // Si usas ObjectFactory
    [SerializeField] private GameObject _prefabToSpawn; // Si usas Instantiate

    [Range(0f, 100f)]
    [Tooltip("Probabilidad de que se spawnee (0 a 100).")]
    [SerializeField] private float _spawnChance = 50f;

    [Header("Positioning")]
    [SerializeField] private Vector3 _spawnOffset = Vector3.zero;

    // Esta función la llamará el DamageSystem
    public void TrySpawn()
    {
        // Tiramos el "dado"
        float randomValue = Random.Range(0f, 100f);
        if (randomValue > _spawnChance) return;

        Vector3 spawnPos = transform.position + _spawnOffset;

        // Spawneamos (usa tu ObjectFactory o Instantiate)
        if (!string.IsNullOrEmpty(_poolIdToSpawn))
        {
            // Si tienes un sistema genérico de pool, úsalo aquí
            // ObjectFactory.Instance.Get<GameObject>(_poolIdToSpawn).transform.position = spawnPos;
        }
        else if (_prefabToSpawn != null)
        {
            Instantiate(_prefabToSpawn, spawnPos, Quaternion.identity);
        }
    }
}
