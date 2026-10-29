using System;
using UnityEngine;
using FactoryPool;
using Patterns.Observer.EventManager_Delegates; // <-- esto arriba del todo

public class Enemy_DamageSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int _damageAmount;

    [Header("Impact Behavior")]
    [Tooltip("Si está activado, este objeto se destruirá (o devolverá al pool) tras chocar con un objetivo válido.")]
    [SerializeField] private bool _destroyOnImpact = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. ¿Se dispara el trigger con algo?
        Debug.Log($"TRIGGER DETECTADO con: {other.gameObject.name}");

        // 2. ¿Las capas coinciden?
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Debug.Log($"La capa SÍ coincide. Aplicando daño...");

            if (other.gameObject.TryGetComponent(out IPlayerHitable hitable))
            {
                hitable.Hit(_damageAmount, gameObject);
                Debug.Log("Daño aplicado con éxito");
            }

            if (_destroyOnImpact)
            {
                Debug.Log("Intentando destruir o devolver al pool...");
                ReturnOrDestroy();
            }
        }
        else
        {
            Debug.LogWarning($"Fallo de capa. La capa del objeto es {LayerMask.LayerToName(other.gameObject.layer)} pero buscas {LayerMask.LayerToName(LayerMask.NameToLayer("TuLayerObjetivo"))}");
        }
    }

    // Método que decide si devolver al pool o destruir
    private void ReturnOrDestroy()
    {
        // ¿Este objeto pertenece a un pool?
        if (TryGetComponent(out Enemy_SpawnOnImpact spawner))
        {
            spawner.TrySpawn();
        }

        if (TryGetComponent(out IPoolable poolable))
        {
            ObjectFactory.Instance.Return(poolable);
            Debug.Log("Destroyed2");

        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Destroyed");
        }
    }
}
