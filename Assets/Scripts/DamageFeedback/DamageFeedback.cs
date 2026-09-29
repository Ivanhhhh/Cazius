using Patterns.Observer.EventManager_Delegates;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    private bool _knockbackInProgress;


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

    
    private IEnumerator MovingLerp(float time, float backAmount)
    {
        _PlayerMovement.enabled = false;
        _gameInputManager.DisablePlayerMovement();

        Vector3 start = _rb.position;
        Vector3 end = start - transform.forward * backAmount;

        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            Vector3 target = Vector3.Lerp(start, end, t);
            target.y = _rb.position.y;
            _rb.MovePosition(target);
            yield return new WaitForFixedUpdate();
        }

        _gameInputManager.EnablePlayerMovement();
        _PlayerMovement.enabled = true;
        _knockbackInProgress = false;

    }


    public void StopMovingMethod(params object[] parameters)
    {
        if (_knockbackInProgress) return;
        _knockbackInProgress = true;
        Debug.Log("Ejecutado");

        _animator.SetTrigger("TakeDamage");

        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.HurtedSFX);
       
         StartCoroutine(MovingLerp(TimeStop,BackAmount));
        
         BloodEffect.transform.position = transform.position;
         BloodEffect.Play();
    }

    public void ParryAttack(params object[] parameters)
    {
        if (_knockbackInProgress) return;
        _knockbackInProgress = true;
        Debug.Log("Ejecutado");

        StartCoroutine(MovingLerp(TimeStop,BackAmount));
    }
}
