using UnityEngine;

public class BuildingFallDestroy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;

    [Header("Initial Shake")]
    [SerializeField] private float _initialShakeStrength = 0.05f;
    [SerializeField] private float _initialShakeDuration = 2f;

    [Header("Impact Shake")]
    [SerializeField] private float _impactShakeStrength = 0.3f;
    [SerializeField] private float _impactShakeDuration = 2f;

    [Header("Destruction Particles")]
    [SerializeField] private ParticleSystem[] _initialParticles;
    [SerializeField] private ParticleSystem[] _impactParticles;

    private bool _hasTriggered = false;
    private bool _impactTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_hasTriggered)
            return;

        _hasTriggered = true;

        // Start building destruction animation
        _animator.SetTrigger("Destroy");
    }

    // ANIMATION EVENT: Building starts collapsing
    public void AnimationEvent_InitialDestruction()
    {
        CameraShakeManager.Instance.CameraShake(
            _initialShakeStrength,
            _initialShakeDuration
        );

        PlayParticles(_initialParticles);
    }

    // ANIMATION EVENT: Building impacts the ground
    public void AnimationEvent_Impact()
    {
        if (_impactTriggered)
            return;

        _impactTriggered = true;

        CameraShakeManager.Instance.CameraShake(
            _impactShakeStrength,
            _impactShakeDuration
        );

        PlayParticles(_impactParticles);
    }

    private void PlayParticles(ParticleSystem[] particles)
    {
        foreach (ParticleSystem particle in particles)
        {
            if (particle == null)
                continue;

            particle.Play();
        }
    }
}