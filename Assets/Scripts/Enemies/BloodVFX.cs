using UnityEngine;

public class BloodVFX : MonoBehaviour
{
    [Header("Blood Particles")]
    [SerializeField] private ParticleSystem _bloodParticles;
    [SerializeField] private Transform _positionSpawnBlood;

    public void BloodDead()
    {
        ParticleSystem blood = Instantiate(_bloodParticles,
                                            _positionSpawnBlood.position,
                                            Quaternion.identity);

        blood.Play();

        Destroy(blood.gameObject, blood.main.duration + blood.main.startLifetime.constantMax);
    }
}
