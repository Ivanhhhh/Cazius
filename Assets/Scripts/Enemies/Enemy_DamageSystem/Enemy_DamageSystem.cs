using System;
using UnityEngine;

public class Enemy_DamageSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int _damageAmount;    

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // 2. Intentamos obtener la interfaz del objeto chocado
            if (other.gameObject.TryGetComponent(out IPlayerHitable hitable))
            {
                hitable.Hit( _damageAmount);
                Debug.Log("aplicar daño");
            }
        }
    }
}
