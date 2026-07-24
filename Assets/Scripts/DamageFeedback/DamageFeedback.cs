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
        //[SerializeField] GameObject Blood;

    //[SerializeField] float ForceAmount;
    private PlayerMovement _PlayerMovement;
    //private Vector3 Back;

    
    void Start()
    {
      //Blood.SetActive(false);
         _rb = GetComponent<Rigidbody>();

        //StartCoroutine(MovingLerp());
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

    // // Update is called once per frame
    // void Update()
    // {
    

    // } 

    public IEnumerator StopMoving()
    {   
        //_PlayerMovement.enabled  = false;
        Debug.Log("Ejecutado");
        //poner animacion;
        //GameInputManager.Instance.DisablePlayerMovement();
        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.HurtedSFX);
       
        //StartCoroutine(MovingLerp(transform.position, transform.position - transform.forward  * BackAmount));
        StartCoroutine(MovingLerp());
         //Blood.SetActive(true);
         //BloodP.SetActive(true);
         BloodEffect.Play();

        //transform.position -= BackAmount;
        //SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.HurtedSFX, transform.position);
        yield return null;
        
        //BloodP.SetActive(false);
        
        
        //GameInputManager.Instance.EnablePlayerMovement();
    }


    private IEnumerator MovingLerp()
    {
      //float t = 0;

      //while (t < 1)
      //{
        _PlayerMovement.enabled = false;
        //float currentForce = Mathf.Lerp(ForceAmount, 0f, t);

        _rb.AddForce(-transform.forward * BackAmount, ForceMode.VelocityChange);

        //t += Time.fixedDeltaTime;

        //yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(TimeStop);

        _PlayerMovement.enabled = true;

      //}
    }

    //private IEnumerator MovingLerp(Vector3 origen, Vector3 destino)
    //{
        //float t = 0;
       
        //while (t < 1)
        //{
          //transform.position = Vector3.Lerp(origen, destino, t);

          //t += Time.deltaTime;

          //yield return null;
        //}
    //}

   

    public void StopMovingMethod(params object[] parameters)
    {
        StartCoroutine(StopMoving());
    }
}
