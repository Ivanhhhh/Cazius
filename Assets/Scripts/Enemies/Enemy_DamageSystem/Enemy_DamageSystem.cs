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
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            if (other.gameObject.TryGetComponent(out IPlayerHitable hitable))
            {
                hitable.Hit(_damageAmount);
                Debug.Log("aplicar daño");
            }

            if (_destroyOnImpact)
            {
                ReturnOrDestroy();
            }
        }
    }

    // Método que decide si devolver al pool o destruir
    private void ReturnOrDestroy()
    {
        // ¿Este objeto pertenece a un pool?
        if (TryGetComponent(out IPoolable poolable))
        {
            ObjectFactory.Instance.Return(poolable);
        }
        else
        {
            // No es pooleable, se destruye como siempre
            Destroy(gameObject);
        }
    }
}
