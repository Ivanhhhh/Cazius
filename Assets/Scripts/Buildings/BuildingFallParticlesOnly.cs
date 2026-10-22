using UnityEngine;

public class BuildingFallParticlesOnly : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Destruction Particles")]
    [SerializeField] private ParticleSystem[] _initialParticles;
    [SerializeField] private ParticleSystem[] _impactParticles;
    [SerializeField] private ParticleSystem[] _impactDustParticles;

    private bool _initialTriggered = false;
    private bool _impactTriggered = false;
    private bool _impactDustTriggered = false;


    private void Awake()
    {
        _animator.keepAnimatorStateOnDisable = true;
        _animator.writeDefaultValuesOnDisable = false;
    }

    public void AnimationEvent_InitialDestruction()
    {
        if (_initialTriggered)
            return;

        _initialTriggered = true;
        PlayParticles(_initialParticles);
    }

    public void AnimationEvent_Impact()
    {
        if (_impactTriggered)
            return;

        _impactTriggered = true;
        PlayParticles(_impactParticles);
    }

    public void AnimationEvent_ImpactDust()
    {
        if (_impactDustTriggered)
            return;

        _impactDustTriggered = true;
        PlayParticles(_impactDustParticles);
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
