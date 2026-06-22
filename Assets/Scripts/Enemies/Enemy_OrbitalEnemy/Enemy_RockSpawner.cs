using UnityEngine;

public class OrbitalSpawner : MonoBehaviour
{
    [Header("Configuración de Spawner")]
    [SerializeField] private GameObject _projectilePrefab; 
    [SerializeField] private Transform _spawnPoint;        
    [SerializeField] private OrbitManager _orbitManager;   
    
    [Header("Conexión de Eventos")]
    [SerializeField] private Enemy_OrbitEnemyData _enemyData; 
    
    [Header("Ajustes de Aparición")]
    [SerializeField] private int _amountToSpawn = 3;

    private void OnEnable()
    {
        if (_enemyData != null)
        {
            // Nos suscribimos al evento C# puro
            _enemyData.OnRequireProjectiles += SpawnProjectiles;
        }
    }

    private void OnDisable()
    {
        if (_enemyData != null)
        {
            // Nos desuscribimos para evitar fugas de memoria
            _enemyData.OnRequireProjectiles -= SpawnProjectiles;
        }
    }

    public void SpawnProjectiles()
    {
        if (_projectilePrefab == null || _orbitManager == null) return;

        Debug.Log($"[Spawner] Generando {_amountToSpawn} proyectiles orbitales...");

        for (int i = 0; i < _amountToSpawn; i++)
        {
            Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            GameObject newProj = Instantiate(_projectilePrefab, spawnPos, Quaternion.identity);
            
            OrbitMovement orbitalComp = newProj.GetComponent<OrbitMovement>();
            if (orbitalComp != null)
            {
                _orbitManager.AddProjectileToOrbit(orbitalComp);
            }
        }

        _orbitManager.RearrangeOrbit();
    }
}
