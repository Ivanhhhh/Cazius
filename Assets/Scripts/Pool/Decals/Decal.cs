using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryPool
{
    public class Decal : MonoBehaviour
    {
       [SerializeField] private float _decalLifetime = 8f;

        float _currentLifeTime;

        private void OnEnable()
        {
            _currentLifeTime = _decalLifetime;
        }

        private void Update()
        {
            _currentLifeTime -= Time.deltaTime;

            if (_currentLifeTime <= 0)
            {
                DecalFactory.Instance.ReturnBullet(this);
            }
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    DecalFactory.Instance.ReturnBullet(this);
        //}
    }
}
