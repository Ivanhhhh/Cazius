using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryPool
{
    public class ClientDebuggerDecal : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var Dec = DecalFactory.Instance.GetDecal();

                Dec.transform.position = Vector3.one * Random.Range(-5, 5);
            }
        }
    }

}
