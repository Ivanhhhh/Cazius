using UnityEngine;

public class BloodVFX : MonoBehaviour
{
    [Header("Blood Particles")]
    [SerializeField] private ParticleSystem _bloodParticles;

    public void BloodDead()
    {
        ParticleSystem blood = Instantiate(_bloodParticles,
                                            transform.position,
                                            Quaternion.identity);

        blood.Play();

        Destroy(blood.gameObject, blood.main.duration + blood.main.startLifetime.constantMax);
    }
}
