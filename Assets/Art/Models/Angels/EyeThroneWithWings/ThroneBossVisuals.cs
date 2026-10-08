using System.Collections;
using UnityEngine;

public class ThroneBossVisuals : MonoBehaviour
{
    [SerializeField] MeshRenderer _eyeRenderer;
    [SerializeField] SkinnedMeshRenderer[] _wingRenderers;
    [SerializeField] Animator _anim;

    private Material _eyeMat;
    private Material[] _wingMats;

    [Header("Eye Damage")]
    [SerializeField] float _eyeDamageDuration = 0.5f;

    [Header("Spawn")]
    [SerializeField] float _phaseChangeDelay = 1.0f;
    [SerializeField] float _dissolveDuration = 1.0f;
    [SerializeField] float _phaseChangeDuration = 1.0f;

    [Header("Despawn")]
    [SerializeField] float _despawnChangeDelay = 1.0f;
    [SerializeField] float _despawnLastDelay = 1.0f;

    [Header("Eye Pulse")]
    [SerializeField] float _pulseSpeed = 2.0f;

    private Coroutine _eyePulseCoroutine;


    private void Awake()
    {
        _eyeMat = _eyeRenderer.material;

        _wingMats = new Material[_wingRenderers.Length];

        for (int i = 0; i < _wingRenderers.Length; i++)
            _wingMats[i] = _wingRenderers[i].material;

    }


    public void EyeTakeDamage()
    {
        StartCoroutine(EyeDamageCoroutine());
    }


    public void StartChanneling()
    {
        _anim.SetBool("IsChanneling", true);
    }


    public void StopChanneling()
    {
        _anim.SetBool("IsChanneling", false);
    }


    public void Spawn()
    {
        StartCoroutine(SpawnCoroutine());
    }


    public void Despawn()
    {
        StartCoroutine(DespawnCoroutine());
    }


    private IEnumerator EyeDamageCoroutine()
    {
        float halfDuration = _eyeDamageDuration / 2f;

        for (float t = 0f; t < halfDuration; t += Time.deltaTime)
        {
            float value = Mathf.Lerp(0f, 1f, t / halfDuration);
            _eyeMat.SetFloat("_EmissiveAmount", value);

            yield return null;
        }

        _eyeMat.SetFloat("_EmissiveAmount", 1f);

        for (float t = 0f; t < halfDuration; t += Time.deltaTime)
        {
            float value = Mathf.Lerp(1f, 0f, t / halfDuration);
            _eyeMat.SetFloat("_EmissiveAmount", value);

            yield return null;
        }

        _eyeMat.SetFloat("_EmissiveAmount", 0f);
    }


    private IEnumerator SpawnCoroutine()
    {
        _anim.SetBool("IsChanneling", true);

        StartEyePulse();

        yield return new WaitForSeconds(_phaseChangeDelay);

        yield return DissolveWings(1f, 0f);

        yield return new WaitForSeconds(_phaseChangeDuration);

        _anim.SetBool("IsChanneling", false);

        StopEyePulse();
    }


    private IEnumerator DespawnCoroutine()
    {
        _anim.SetBool("IsChanneling", true);

        StartEyePulse();

        yield return new WaitForSeconds(_despawnChangeDelay);

        yield return DissolveWings(0f, 1f);

        yield return new WaitForSeconds(_despawnLastDelay);

        _anim.SetBool("IsChanneling", false);

        StopEyePulse();
    }


    private IEnumerator DissolveWings(float startValue, float endValue)
    {
        for (float t = 0f; t < _dissolveDuration; t += Time.deltaTime)
        {
            float value = Mathf.Lerp(startValue, endValue, t / _dissolveDuration);

            _eyeMat.SetFloat("_DissolveAmount", value);

            foreach (var mat in _wingMats)
                mat.SetFloat("_DissolveAmount", value);

            yield return null;
        }

        _eyeMat.SetFloat("_DissolveAmount", endValue);

        foreach (var mat in _wingMats)
            mat.SetFloat("_DissolveAmount", endValue);
    }


    private void StartEyePulse()
    {
        if (_eyePulseCoroutine != null)
            StopCoroutine(_eyePulseCoroutine);

        _eyePulseCoroutine = StartCoroutine(EyePulseCoroutine());
    }


    private void StopEyePulse()
    {
        if (_eyePulseCoroutine != null)
        {
            StopCoroutine(_eyePulseCoroutine);
            _eyePulseCoroutine = null;
        }

        _eyeMat.SetFloat("_EmissiveAmount", 0f);
    }


    private IEnumerator EyePulseCoroutine()
    {
        while (true)
        {
            float value = Mathf.PingPong(Time.time * _pulseSpeed, 1f);

            _eyeMat.SetFloat("_EmissiveAmount", value);

            yield return null;
        }
    }
}