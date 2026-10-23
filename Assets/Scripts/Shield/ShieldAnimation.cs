using System.Collections;
using UnityEngine;

public class ShieldAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animatior;
    [SerializeField] private float _soundStrength = 1f;

    [Header("Initial Shake")]
    [SerializeField] private float _initialShakeStrength = 0.05f;
    [SerializeField] private float _initialShakeDuration = 5f;

    [Header("Impact Shake")]
    [SerializeField] private float _impactShakeStrength = 0.3f;
    [SerializeField] private float _impactShakeDuration = 5f;

    [Header("Destruction Particles")]
    [SerializeField] private ParticleSystem _impactDustParticles;


    private bool isEnable = false;

    void OnTriggerEnter(Collider other)
    {
        if (isEnable) return;

        if (other.CompareTag("Player"))
        {
            CameraShakeManager.Instance.CameraShake(_initialShakeStrength, _initialShakeDuration);

            isEnable = true;

            StartCoroutine(waitEffects());

            animatior.SetTrigger("Enable");
        }
    }



    IEnumerator waitEffects()
    {
        yield return new WaitForSeconds(1.5f);

        _impactDustParticles.Play();

        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.RockCrumble, _soundStrength);
    }
}
