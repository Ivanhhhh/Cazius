using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Patterns.Observer.EventManager_Delegates;
using UnityEngine.VFX;

public class DamageFeedback : MonoBehaviour
{
    [SerializeField] float TimeStop;
    [SerializeField] float BackAmount;
    [SerializeField] Rigidbody _rb;
    [SerializeField] VisualEffect BloodEffect;

    private PlayerMovement _PlayerMovement;
    
    void Start()
    {
         _rb = GetComponent<Rigidbody>();

        _PlayerMovement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
      EventManager.SubscribeToEvent(EventsType.Event_PausePlayer, StopMovingMethod);
    }

    void OnDisable()
    {
      EventManager.UnsubscribeToEvent(EventsType.Event_PausePlayer,StopMovingMethod);
    }

    
    private IEnumerator MovingLerp()
    {
        _PlayerMovement.enabled = false;

        _rb.linearVelocity = Vector3.zero;
        _rb.AddForce(-transform.forward * BackAmount, ForceMode.VelocityChange);

        yield return new WaitForSeconds(TimeStop);

        _PlayerMovement.enabled = true;
    }

    
    public void StopMovingMethod(params object[] parameters)
    {
         Debug.Log("Ejecutado");
        //poner animacion;
        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.HurtedSFX);
       
        StartCoroutine(MovingLerp());
        
         BloodEffect.Play();
    }
}
