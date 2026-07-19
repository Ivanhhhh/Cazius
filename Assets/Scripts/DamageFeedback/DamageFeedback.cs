using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Patterns.Observer.EventManager_Delegates;


public class DamageFeedback : MonoBehaviour
{
    public delegate void DmgFeedback(params object[] parameters);
    private DmgFeedback _DmgFeedback;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // _DmgFeedback += StopMovingMethod;

        
    }

    void OnEnable()
    {
       EventManager.SubscribeToEvent(EventsType.Event_PausePlayer,StopMovingMethod);
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
        GameInputManager.Instance.EnableDialogInput();
        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.HurtedSFX);
        yield return new WaitForSeconds(2);
        GameInputManager.Instance.DisableDialogInput();
    }

    public void StopMovingMethod(params object[] parameters)
    {
        StartCoroutine(StopMoving());
    }
}
