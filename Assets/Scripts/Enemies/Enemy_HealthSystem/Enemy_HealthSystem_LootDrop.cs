using System.Collections;
using UnityEngine;

/// <summary>
/// Maneja el drop de loot al morir (soul energy, items, etc).
/// Tiene su propio delay, independiente del deathDelay del health system,
/// para no acoplar el timing del loot con el timing de destruccion del enemigo.
/// </summary>
[RequireComponent(typeof(Enemy_HealthSystem_Base))]
public class Enemy_HealthSystem_LootDrop : MonoBehaviour
{
    public enum LootDropType
    {
        None,
        ActivateExistingObject,
        InstantiatePrefab
    }

    [SerializeField] private LootDropType _lootDropType = LootDropType.None;

    [Tooltip("Si es ActivateExistingObject: objeto ya en escena que se activa. Si es InstantiatePrefab: prefab a instanciar.")]
    [SerializeField] private GameObject _lootObject;

    [Tooltip("Punto de spawn del loot. Si esta vacio, usa la posicion del enemigo.")]
    [SerializeField] private Transform _lootSpawnPoint;

    [Tooltip("Delay propio antes de soltar el loot, independiente del delay de muerte del health system.")]
    [SerializeField] private float _dropDelay = 0f;

    private Enemy_HealthSystem_Base _healthSystem;

    void Awake()
    {
        _healthSystem = GetComponent<Enemy_HealthSystem_Base>();
    }

    void OnEnable()
    {
        _healthSystem.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        _healthSystem.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (_lootDropType == LootDropType.None) return;

        StartCoroutine(DropRoutine());
    }

    private IEnumerator DropRoutine()
    {
        if (_dropDelay > 0f)
            yield return new WaitForSeconds(_dropDelay);

        Vector3 spawnPos = _lootSpawnPoint != null ? _lootSpawnPoint.position : transform.position;

        switch (_lootDropType)
        {
            case LootDropType.ActivateExistingObject:
                if (_lootObject != null)
                {
                    _lootObject.SetActive(true);
                    _lootObject.transform.position = spawnPos;
                }
                break;

            case LootDropType.InstantiatePrefab:
                if (_lootObject != null)
                    Instantiate(_lootObject, spawnPos, Quaternion.identity);
                break;
        }
    }
}
