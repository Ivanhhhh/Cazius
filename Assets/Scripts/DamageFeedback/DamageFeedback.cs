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
    [SerializeField] Animator _animator;
    private GameInputManager _gameInputManager;

    private Animator _Animator;

    private PlayerMovement _PlayerMovement;

    [SerializeField] Vector3 BloodPosition;
    
    void Start()
    {
         _rb = GetComponent<Rigidbody>();

        _PlayerMovement = GetComponent<PlayerMovement>();

        _Animator = GetComponent<Animator>();
            
        _gameInputManager = GameInputManager.Instance;
    }

    void OnEnable()
    {
      EventManager.SubscribeToEvent(EventsType.Event_PausePlayer, StopMovingMethod);
      

        BloodEffect.transform.position = transform.up * 9; // solo el componente, no el gameObject
    }

    void OnDisable()
    {
      EventManager.UnsubscribeToEvent(EventsType.Event_PausePlayer,StopMovingMethod);
    }

    
    private IEnumerator MovingLerp(float time)
    {

        _PlayerMovement.enabled = false;
        _gameInputManager.DisablePlayerMovement();

                 //poner animacion;
        _rb.linearVelocity = Vector3.zero;
        _rb.AddForce(-transform.forward * BackAmount, ForceMode.VelocityChange);

        yield return new WaitForSeconds(time);
        _gameInputManager.EnablePlayerMovement();
        _PlayerMovement.enabled = true;
    }

    
    public void StopMovingMethod(params object[] parameters)
    {
        Debug.Log("Ejecutado");

        _animator.SetTrigger("TakeDamage");

        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.HurtedSFX);
       
         StartCoroutine(MovingLerp(TimeStop));
        
         BloodEffect.transform.position = transform.position;
         BloodEffect.Play();
    }

    public void ParryAttack(params object[] parameters)
    {
        Debug.Log("Ejecutado");

        StartCoroutine(MovingLerp(0.6f));
    }
}
