using System;
using UnityEngine;

public class RainingRocks : MonoBehaviour
{
    public enum ActionType
    {
        Sound,
        Animation
    }

    [SerializeField] private ActionType actionType;

    [Header("Sound")]
    [SerializeField] private SFXManager.SFXCategoryType sfxType;

    [Header("Animtor")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (actionType == ActionType.Sound)
        {
            SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(sfxType, transform.position);
        }
        else if (actionType == ActionType.Animation)
        {
            animator.SetTrigger(animationTrigger);
        }
    }

}
