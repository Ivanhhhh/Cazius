using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryPool
{
    public class BulletFactory : MonoBehaviour
    {
        public static BulletFactory Instance { get; private set; }

        [System.Serializable]
        public struct BulletConfig
        {
            public BulletType type;
            public Bullet prefab;
            public int initialAmount;
        }

        [SerializeField] private List<BulletConfig> _bulletConfigs;
        [SerializeField] private Transform _container;

        // Un diccionario que guarda un Pool por cada tipo de bala
        private Dictionary<BulletType, Pool<Bullet>> _pools = new Dictionary<BulletType, Pool<Bullet>>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (_container == null) _container = this.transform;

            // Creamos un Pool para cada configuración
            foreach (var config in _bulletConfigs)
            {
                // Usamos una variable local para evitar el problema de captura de variables en el closure
                Bullet prefabLocal = config.prefab;

                Pool<Bullet> newPool = new Pool<Bullet>(
                    factoryMethod: () => Instantiate(prefabLocal, _container),
                    turnOnCallback: (b) =>  b.gameObject.SetActive(true),
                    turnOffCallback: (b) => b.gameObject.SetActive(false),
                    initialAmount: config.initialAmount
                );

                _pools.Add(config.type, newPool);
            }
        }

        public Bullet GetBullet(BulletType type)
        {
            if (_pools.TryGetValue(type, out Pool<Bullet> pool))
            {
                return pool.GetObject();
            }

            Debug.LogError($"No se encontró un pool para el tipo de bala: {type}");
            return null;
        }

        public void ReturnBullet(BulletType type, Bullet bullet)
        {
            if (_pools.TryGetValue(type, out Pool<Bullet> pool))
            {
                pool.ReturnObjectToPool(bullet);
            }
        }
    }
}
public enum BulletType
{
    Caster,
    Orbit,
    SoulEnergy
}
