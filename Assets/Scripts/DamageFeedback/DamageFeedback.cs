using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Patterns.Observer.EventManager_Delegates;


public class DamageFeedback : MonoBehaviour
{
    [SerializeField] float Time;

    [SerializeField] PlayerMovement _pl;
    
   // public delegate void DmgFeedback(params object[] parameters);
   // private DmgFeedback _DmgFeedback;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //StartCoroutine(StopMoving());
    }

    void OnEnable()
    {
    EventManager.SubscribeToEvent(EventsType.Event_PausePlayer, StopMovingMethod);
    }

    void OnDisable()
    {
      EventManager.UnsubscribeToEvent(EventsType.Event_PausePlayer,StopMovingMethod);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator StopMoving()
    {   
        Debug.Log("Ejecutado");
        //poner animacion;
        GameInputManager.Instance.DisablePlayerMovement();
        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.HurtedSFX);
        //SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.HurtedSFX, transform.position);
        yield return new WaitForSeconds(Time);
        GameInputManager.Instance.EnablePlayerMovement();
    }

   

    public void StopMovingMethod(params object[] parameters)
    {
        StartCoroutine(StopMoving());
    }
}
