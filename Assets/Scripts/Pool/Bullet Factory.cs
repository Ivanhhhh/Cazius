using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryPool
{
    public class BulletFactory : MonoBehaviour
    {
        public static BulletFactory Instance { get; private set; }

        [SerializeField] Bullet _bulletPrefab;

        Pool<Bullet> _pool;

        private void Awake()
        {
            Instance = this;

            _pool = new Pool<Bullet>(CreateObject, TurnOn, TurnOff, 10);
        }

        Bullet CreateObject()
        {
            var result = Instantiate(_bulletPrefab);
            return result;
        }

        void TurnOn(Bullet b)
        {
            b.gameObject.SetActive(true);
        }

        void TurnOff(Bullet b)
        {
            b.gameObject.SetActive(false);
        }

        public Bullet GetBullet()
        {
            return _pool.GetObject();
        }

        public void ReturnBullet(Bullet bullet)
        {
            _pool.ReturnObjectToPool(bullet);
        }
    }

}
