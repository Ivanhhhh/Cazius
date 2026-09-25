using System.Collections.Generic;
using UnityEngine;

namespace FactoryPool
{
    public class ObjectFactory : MonoBehaviour
    {
        public static ObjectFactory Instance { get; private set; }

        [SerializeField] private List<PoolConfigSO> _configs;
        [SerializeField] private Transform _container;

        private Dictionary<string, Pool<MonoBehaviour>> _pools = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (_container == null) _container = this.transform;

            foreach (var config in _configs)
            {
                if (config == null || config.prefab == null)
                {
                    Debug.LogWarning("PoolConfig nulo o sin prefab, ignorando.");
                    continue;
                }

                var prefabLocal = config.prefab;
                var idLocal = config.id;

                Pool<MonoBehaviour> newPool = new Pool<MonoBehaviour>(
                    factoryMethod: () =>
                    {
                        var obj = Instantiate(prefabLocal, _container);
                        if (obj is IPoolable poolable)
                            poolable.PoolId = idLocal;
                        else
                            Debug.LogError($"{obj.name} no implementa IPoolable");
                        return obj;
                    },
                    turnOnCallback: (obj) =>
                    {
                        if (obj is IPoolable poolable) poolable.OnGetFromPool();
                    },
                    turnOffCallback: (obj) =>
                    {
                        if (obj is IPoolable poolable) poolable.OnReturnToPool();
                    },
                    initialAmount: config.initialAmount
                );

                _pools.Add(idLocal, newPool);
            }
        }

        // Devuelve un MonoBehaviour (tú harás el cast al tipo que necesites)
        public MonoBehaviour Get(string id)
        {
            if (_pools.TryGetValue(id, out var pool))
                return pool.GetObject();

            Debug.LogError($"No existe un pool con id: {id}");
            return null;
        }

        // Versión genérica opcional, para evitar casts en el cliente
        public T Get<T>(string id) where T : MonoBehaviour
        {
            return Get(id) as T;
        }

        // El objeto sabe a qué pool volver gracias a su PoolId
        public void Return(IPoolable obj)
        {
            if (obj == null) return;

            if (_pools.TryGetValue(obj.PoolId, out var pool))
                pool.ReturnObjectToPool(obj as MonoBehaviour);
            else
                Debug.LogError($"Pool no encontrado para id: {obj.PoolId}");
        }
    }
}
