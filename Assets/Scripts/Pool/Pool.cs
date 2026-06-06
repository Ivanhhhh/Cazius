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

        List<T> _currentStock; //Mi "cajon" donde voy a guardar los objetos disponibles para su uso

        public Pool(Func<T> factoryMethod, Action<T> turnOnCallback, Action<T> turnOffCallback, int initialAmount)
        {
            _factoryMethod = factoryMethod;
            _turnOnCallback = turnOnCallback;
            _turnOffCallback = turnOffCallback;

            _currentStock = new List<T>();

            for (int i = 0; i < initialAmount; i++)
            {
                var createdObject = _factoryMethod();

                _turnOffCallback(createdObject);

                _currentStock.Add(createdObject);

            }
        }

        public T GetObject()
        {
            T objectToReturn;

            //Si tengo algo en la lista
            if (_currentStock.Count != 0)
            {
                //lo obtengo de la lista
                objectToReturn = _currentStock[0];

                //lo saco de la lista
                _currentStock.RemoveAt(0);
            }
            else //Si no tengo algo en la lista
            {
                //lo creo
                objectToReturn = _factoryMethod();
            }

            //lo prendo
            _turnOnCallback(objectToReturn);

            //lo devuelvo
            return objectToReturn;
        }

        public void ReturnObjectToPool(T obj)
        {
            //Lo apago
            _turnOffCallback(obj);

            //Lo agrego a la lista
            _currentStock.Add(obj);
        }
    }
}