using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryPool
{
    public class ClientDebugger : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var bullet = BulletFactory.Instance.GetBullet();

                bullet.transform.position = Vector3.one * Random.Range(-5, 5);
            }
        }
    }

}
