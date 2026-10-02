using System.Collections;
using UnityEngine;

public class ParticleAutoStop : MonoBehaviour
{
    [SerializeField] ParticleSystem _particles;
    /*[SerializeField] float _playDuration = 7f;

    private Coroutine _stopCoroutine;

    private void OnEnable()
    {
        _particles.Play();

        if (_stopCoroutine != null)
            StopCoroutine(_stopCoroutine);

        _stopCoroutine = StartCoroutine(StopParticlesCoroutine());
    }

    private IEnumerator StopParticlesCoroutine()
    {
        yield return new WaitForSeconds(_playDuration);

        _particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        _stopCoroutine = null;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}