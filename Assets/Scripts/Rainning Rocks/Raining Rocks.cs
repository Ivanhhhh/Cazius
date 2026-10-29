
using UnityEngine;
using System.Collections;

public class RainingRocks : MonoBehaviour
{
    public enum ActionType
    {
        Sound,
        Animator
    }

    [SerializeField] private ActionType actionType;

    [Header("Sound")]
    [SerializeField] private SFXManager.SFXCategoryType sfxType;

    [Header("Animators")]
    [SerializeField] private Animator[] animators;

    [Header("Initial Shake")]
    [SerializeField] private float _initialShakeStrength = 0.05f;
    [SerializeField] private float _initialShakeDuration = 5f;

    [Header("Impact Shake")]
    [SerializeField] private float _impactShakeStrength = 0.3f;
    [SerializeField] private float _impactShakeDuration = 5f;

    [Header("Destruction Particles")]
    [SerializeField] private ParticleSystem[] _impactDustParticles;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggered) return;

        triggered = true;

        if (actionType == ActionType.Sound)
        {
            SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(
                sfxType, transform.position
            );

            CameraShakeManager.Instance.CameraShake(
                _initialShakeStrength,
                _initialShakeDuration
            );
            foreach (Animator anim in animators)
            {
                if (anim != null)
                {
                    anim.SetTrigger("Enable");
                }
            }

            foreach (ParticleSystem particle in _impactDustParticles)
            {
                if (particle != null)
                {
                    particle.Play();
                }
            }
        }
        else if (actionType == ActionType.Animator)
        {
            CameraShakeManager.Instance.CameraShake(
                _initialShakeStrength,
                _initialShakeDuration
            );

            SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(
                          sfxType, transform.position
                      );

            foreach (ParticleSystem particle in _impactDustParticles)
            {
                if (particle != null)
                {
                    particle.Play();
                }
            }

            foreach (Animator anim in animators)
            {
                if (anim != null)
                {
                    anim.SetTrigger("Enable");
                }
            }
        }
    }
}
