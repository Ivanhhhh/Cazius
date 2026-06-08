using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryPool
{
    public class DecalFactory : MonoBehaviour
    {
        public static DecalFactory Instance { get; private set; }

        [SerializeField] Decal _decalPrefab;

        Pool<Decal> _pool;

        private void Awake()
        {
            Instance = this;

            _pool = new Pool<Decal>(CreateObject, TurnOn, TurnOff, 10);
        }

        Decal CreateObject()
        {
            var result = Instantiate(_decalPrefab);
            return result;
        }

        void TurnOn(Decal b)
        {
            b.gameObject.SetActive(true);
        }

        void TurnOff(Decal b)
        {
            b.gameObject.SetActive(false);
        }

        public Decal GetDecal()
        {
            return _pool.GetObject();
        }

        public void ReturnBullet(Decal bullet)
        {
            _pool.ReturnObjectToPool(bullet);
        }
    }

}
