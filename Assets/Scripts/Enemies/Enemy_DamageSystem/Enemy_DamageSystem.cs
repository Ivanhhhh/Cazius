using System;
using UnityEngine;
using Patterns.Observer.EventManager_Delegates; // <-- esto arriba del todo

public class Enemy_DamageSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int _damageAmount;    

    [Header("Impact Behavior")]
    [Tooltip("Si está activado, este objeto se destruirá tras chocar con un objetivo válido.")]
    [SerializeField] private bool _destroyOnImpact = false;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            if (other.gameObject.TryGetComponent(out IPlayerHitable hitable))
            {
                hitable.Hit(_damageAmount);
                Debug.Log("aplicar daño");
                //EventManager.TriggerEvent(EventsType.Event_PausePlayer);
            }

            if (_destroyOnImpact)
            {
                Destroy(gameObject);
            }
        }
    }
}
