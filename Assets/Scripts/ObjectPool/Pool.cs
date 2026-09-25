using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryPool
{
    public class Pool<T> where T : MonoBehaviour
    {
        Func<T> _factoryMethod; //Aca voy a guardar COMO se crea el objeto

        Action<T> _turnOnCallback; //Aca voy a guardar como se prende el objeto antes de darselo al cliente

        Action<T> _turnOffCallback; //Aca voy a guardar como se apaga el objeto una vez regrese al pool

        Queue<T> _currentStock;
        HashSet<T> _objectsInPool = new HashSet<T>(); // <--- NUEVO


        public Pool(Func<T> factoryMethod, Action<T> turnOnCallback, Action<T> turnOffCallback, int initialAmount)
        {
            _factoryMethod = factoryMethod;
            _turnOnCallback = turnOnCallback;
            _turnOffCallback = turnOffCallback;
            _currentStock = new Queue<T>(initialAmount);

            for (int i = 0; i < initialAmount; i++)
            {
                var createdObject = _factoryMethod();
                _turnOffCallback(createdObject);
                _currentStock.Enqueue(createdObject);
                _objectsInPool.Add(createdObject);
            }
        }

        public T GetObject()
        {
            T objectToReturn;

            if (_currentStock.Count != 0)
            {
                objectToReturn = _currentStock.Dequeue();
            }
            else
            {
                objectToReturn = _factoryMethod();
            }

            _objectsInPool.Remove(objectToReturn); // <--- Ya no está en el pool
            _turnOnCallback(objectToReturn);
            return objectToReturn;
        }

        public void ReturnObjectToPool(T obj)
        {
            // Si ya está en el pool, ignorar (evita doble devolución)
            if (_objectsInPool.Contains(obj)) return;

            _turnOffCallback(obj);
            _currentStock.Enqueue(obj);
            _objectsInPool.Add(obj);
        }
    }
}